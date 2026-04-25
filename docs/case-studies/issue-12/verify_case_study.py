#!/usr/bin/env python3
"""Lightweight consistency checks for the issue #12 case study."""

from __future__ import annotations

import json
from pathlib import Path


ROOT = Path(__file__).resolve().parent
RAW = ROOT / "raw"


def read_json(name: str):
    return json.loads((RAW / name).read_text(encoding="utf-8"))


def main() -> int:
    run = read_json("ci-run-24940817291.json")
    artifacts = read_json("ci-run-24940817291-artifacts.json")

    assert run["event"] == "pull_request"
    assert run["conclusion"] == "success"
    assert artifacts["total_count"] == 0

    jobs = {job["name"]: job for job in run["jobs"]}
    assert jobs["Check for UNITY_LICENSE in GitHub Secrets"]["conclusion"] == "success"
    assert jobs["Request Unity activation file"]["conclusion"] == "skipped"
    assert jobs["Package Windows Shipping Build (StandaloneWindows64)"]["conclusion"] == "skipped"
    assert jobs["Package Windows Shipping Build (StandaloneWindows64)"]["steps"] == []

    log = (RAW / "ci-log-run-24940817291.log").read_text(encoding="utf-8")
    assert "UNITY_LICENSE is NOT set." in log
    assert "no OneTry-Win64 ZIP artifact is uploaded" in log

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

    print("issue-12 case study checks passed")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
