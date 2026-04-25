# References - Issue #17

## Repository evidence

- Issue #17: <https://github.com/Jhon-Crow/One-try/issues/17>
- PR #18: <https://github.com/Jhon-Crow/One-try/pull/18>
- Referenced CI run: <https://github.com/Jhon-Crow/One-try/actions/runs/24941217038>
- Referenced CI job: <https://github.com/Jhon-Crow/One-try/actions/runs/24941217038/job/73035169259>
- Workflow snapshot: [`raw/build.yml.snapshot`](./raw/build.yml.snapshot)
- CI log snapshot: [`raw/ci-log-run-24941217038.log`](./raw/ci-log-run-24941217038.log)

## External references

- Buildalon `activate-unity-license` GitHub Action:
  <https://github.com/buildalon/activate-unity-license>
- Buildalon Actions documentation:
  <https://www.buildalon.com/docs/actions>
- GameCI Unity Builder documentation:
  <https://game.ci/docs/github/builder>
- Unity Support - activate a Unity license:
  <https://support.unity.com/hc/en-us/articles/211438683-How-do-I-activate-my-license>
- Unity Support - command-line license activation:
  <https://support.unity.com/hc/en-us/articles/5541428270356-How-to-activate-a-Unity-license-via-command-line-on-Windows->

## Notes from external research

Buildalon documents `activate-unity-license` as a Unity CI/CD licensing action
and current examples use the `@v2` major version. GameCI documents
`unity-builder@v4` for Unity builds and examples pass Unity credentials through
repository secrets. Unity support documentation confirms Unity license
activation is an explicit prerequisite for using the Editor.
