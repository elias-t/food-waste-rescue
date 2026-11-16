#!/usr/bin/env node
// Stop hook: before Claude says "done", run build/typecheck/tests for the parts of the
// repo that have uncommitted changes. If anything fails, Claude is sent back to fix it
// (exit 2, output on stderr). Gives up after MAX_ATTEMPTS so it can never loop forever.
// What to run lives in checks.config.mjs next to this file; edit that, not this.
import { spawnSync } from "node:child_process";
import { existsSync, readFileSync, writeFileSync, rmSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import { CHECKS } from "./checks.config.mjs";

const MAX_ATTEMPTS = 3;
const isWin = process.platform === "win32";
const projectDir = process.env.CLAUDE_PROJECT_DIR || process.cwd();

let input = {};
try { input = JSON.parse(readFileSync(0, "utf8") || "{}"); } catch {}
const counterFile = path.join(os.tmpdir(), `claude-stopcheck-${input.session_id || "default"}`);

// Which files changed? (uncommitted + untracked). If git isn't available, check everything.
let changed = null;
const git = spawnSync("git", ["status", "--porcelain", "-uall"], { cwd: projectDir, encoding: "utf8" });
if (git.status === 0) {
  changed = git.stdout.split("\n").filter(Boolean)
    .map((l) => l.slice(3).split(" -> ").pop().replace(/^"|"$/g, "").replace(/\\/g, "/"));
  if (changed.length === 0) { rmSync(counterFile, { force: true }); process.exit(0); }
}

const toRun = CHECKS.filter((c) => changed === null || changed.some((f) => c.when.test(f)));
const failures = [];
for (const c of toRun) {
  const cwd = path.join(projectDir, c.cwd || ".");
  if (!existsSync(cwd)) continue;
  const [cmd, ...args] = c.cmd;
  const r = spawnSync(cmd, args, {
    cwd, encoding: "utf8", shell: isWin, timeout: (c.timeoutSec || 300) * 1000,
    env: { ...process.env, CI: "true" }, // stops jest/vitest from starting in watch mode
  });
  if (r.error?.code === "ENOENT") continue; // tool not installed: skip
  if (r.status !== 0) {
    const out = `${r.stdout || ""}${r.stderr || ""}`.trim().split("\n").slice(-80).join("\n");
    failures.push(`## ${c.name} failed (${c.cmd.join(" ")} in ${c.cwd || "."})\n${out}`);
  }
}

if (failures.length === 0) { rmSync(counterFile, { force: true }); process.exit(0); }

let attempts = 0;
try { attempts = Number(readFileSync(counterFile, "utf8")) || 0; } catch {}
if (attempts >= MAX_ATTEMPTS) {
  rmSync(counterFile, { force: true });
  console.log(JSON.stringify({ systemMessage: `Checks still failing after ${MAX_ATTEMPTS} fix attempts: ${failures.map((f) => f.split("\n")[0].slice(3)).join("; ")}` }));
  process.exit(0);
}
writeFileSync(counterFile, String(attempts + 1));
console.error(failures.join("\n\n") + `\n\nThe work is not done: fix the failures above, then finish. (attempt ${attempts + 1}/${MAX_ATTEMPTS})`);
process.exit(2);
