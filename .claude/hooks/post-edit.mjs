#!/usr/bin/env node
// PostToolUse hook: formats + lints ONLY the file Claude just edited.
// Kept fast on purpose; the full build/test run happens in stop-check.mjs.
// Exit 2 = problems Claude must fix (stderr is shown to Claude). Exit 0 = fine.
// A tool that isn't installed is skipped, never treated as a failure.
import { spawnSync } from "node:child_process";
import { existsSync, readFileSync } from "node:fs";
import path from "node:path";

const isWin = process.platform === "win32";
const projectDir = process.env.CLAUDE_PROJECT_DIR || process.cwd();

let input = {};
try { input = JSON.parse(readFileSync(0, "utf8") || "{}"); } catch { process.exit(0); }
const file = input?.tool_input?.file_path;
if (!file || !existsSync(file)) process.exit(0);
const ext = path.extname(file).toLowerCase();

// Walk up from the edited file to find the folder that owns it (package.json, pyproject.toml...).
function findRoot(start, markers) {
  let dir = path.dirname(start);
  while (true) {
    if (markers.some((m) => existsSync(path.join(dir, m)))) return dir;
    const parent = path.dirname(dir);
    if (parent === dir || !dir.startsWith(projectDir)) return null;
    dir = parent;
  }
}

function run(cmd, args, cwd) {
  const r = spawnSync(cmd, args, { cwd, encoding: "utf8", shell: isWin, timeout: 120_000 });
  if (r.error?.code === "ENOENT") return { skipped: true };
  return { code: r.status ?? 1, out: `${r.stdout || ""}${r.stderr || ""}`.trim() };
}

const steps = []; // [label, cmd, args, cwd]

if ([".ts", ".tsx", ".js", ".jsx", ".mjs", ".cjs", ".css", ".scss", ".json"].includes(ext)) {
  const root = findRoot(file, ["package.json"]);
  const bin = (name) => root && existsSync(path.join(root, "node_modules", ".bin", name));
  if (bin("prettier")) steps.push(["prettier", "npx", ["--no-install", "prettier", "--write", "--log-level", "warn", file], root]);
  if (bin("eslint") && ![".css", ".scss", ".json"].includes(ext))
    steps.push(["eslint", "npx", ["--no-install", "eslint", "--fix", file], root]);
}

if (ext === ".py") {
  const root = findRoot(file, ["pyproject.toml", "setup.cfg", "requirements.txt"]) || projectDir;
  const uv = existsSync(path.join(root, "uv.lock"));
  const ruff = (...a) => (uv ? ["uv", ["run", "ruff", ...a]] : ["ruff", a]);
  steps.push(["ruff format", ...ruff("format", file), root]);
  steps.push(["ruff check", ...ruff("check", "--fix", file), root]);
}

if (ext === ".cs") {
  // --folder skips loading the solution, so this stays quick. Compile errors are caught at Stop.
  steps.push(["dotnet format", "dotnet", ["format", "whitespace", "--folder", "--include", path.relative(projectDir, file)], projectDir]);
}

const failures = [];
for (const [label, cmd, args, cwd] of steps) {
  const r = run(cmd, args, cwd);
  if (!r.skipped && r.code !== 0) failures.push(`## ${label} failed for ${path.relative(projectDir, file)}\n${r.out.split("\n").slice(-40).join("\n")}`);
}

if (failures.length) {
  console.error(failures.join("\n\n") + "\n\nFix these issues before moving on.");
  process.exit(2);
}
process.exit(0);
