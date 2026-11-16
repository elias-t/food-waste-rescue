// Checks run by stop-check.mjs when Claude finishes a task.
// `when` = regex on changed file paths (relative to repo root, forward slashes)
// `cwd`  = folder to run in; `cmd` = command + args.
// `npm run --if-present x` silently skips scripts that don't exist.
const fe = /^frontend\//;
const be = /^backend\/.*(\.(cs|csproj|slnx|props|targets)|appsettings.*\.json)$/;

export const CHECKS = [
  { name: "dotnet build", when: be, cwd: "backend",
    cmd: ["dotnet", "build", "FoodWasteRescue.slnx", "--nologo", "-v", "q", "-clp:ErrorsOnly"] },
  { name: "dotnet test", when: be, cwd: "backend",
    cmd: ["dotnet", "test", "FoodWasteRescue.slnx", "--nologo", "-v", "q"], timeoutSec: 600 },

  { name: "frontend typecheck", when: fe, cwd: "frontend",
    cmd: ["npm", "run", "--if-present", "typecheck"] },
  { name: "frontend lint", when: fe, cwd: "frontend",
    cmd: ["npm", "run", "--if-present", "lint"] },
  { name: "frontend tests", when: fe, cwd: "frontend",
    cmd: ["npm", "run", "--if-present", "test"], timeoutSec: 600 },
];
