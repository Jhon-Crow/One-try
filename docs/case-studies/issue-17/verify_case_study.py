#!/usr/bin/env python3
"""Lightweight consistency checks for the issue #17 case study."""

from __future__ import annotations

import json
from pathlib import Path


ROOT = Path(__file__).resolve().parent
RAW = ROOT / "raw"
REPO = ROOT.parents[2]


def read_json(name: str):
    return json.loads((RAW / name).read_text(encoding="utf-8"))


def main() -> int:
    run = read_json("ci-run-24941217038.json")

    assert run["conclusion"] == "success"
    jobs = {job["name"]: job for job in run["jobs"]}
    assert jobs["Check for Unity credentials in GitHub Secrets"]["conclusion"] == "success"
    assert jobs["Package Windows Shipping Build (StandaloneWindows64)"]["conclusion"] == "skipped"
    assert jobs["Package Windows Shipping Build (StandaloneWindows64)"]["steps"] == []

    log = (RAW / "ci-log-run-24941217038.log").read_text(encoding="utf-8")
    assert "UNITY_EMAIL:" in log
    assert "UNITY_PASSWORD:" in log
    assert "Unity credentials (UNITY_EMAIL and/or UNITY_PASSWORD) are NOT set." in log
    assert "no OneTry-Win64 ZIP artifact is uploaded" in log

    workflow = (REPO / ".github/workflows/build.yml").read_text(encoding="utf-8")
    assert "SHOULD_FAIL_WITHOUT_CREDENTIALS" in workflow
    assert "github.event_name == 'workflow_dispatch'" in workflow
    assert "github.ref == 'refs/heads/main'" in workflow
    assert "buildalon/activate-unity-license@v2" in workflow

    required_docs = [
        "README.md",
        "issue.md",
        "timeline.md",
        "root-cause.md",
        "analysis.md",
        "proposed-solutions.md",
        "references.md",
    ]
    for name in required_docs:
        assert (ROOT / name).exists(), name

    print("issue-17 case study checks passed")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
