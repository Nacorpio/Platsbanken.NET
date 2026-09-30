#!/usr/bin/env python3
"""Detect drift in the upstream JobTech OpenAPI specs.

Fetches each spec, normalizes it (sorted keys, stable indentation) and compares it with the
baseline committed under specs/.

    python scripts/check_specs.py            # exit 1 and write specs-diff.md when a spec changed
    python scripts/check_specs.py --update   # refresh the committed baselines
"""
import difflib
import json
import re
import sys
import urllib.request
from pathlib import Path

SPECS = {
    "jobsearch": "https://jobsearch.api.jobtechdev.se/swagger.json",
    "jobstream": "https://jobstream.api.jobtechdev.se/swagger.json",
    "taxonomy": "https://taxonomy.api.jobtechdev.se/v1/taxonomy/openapi.json",
}

ROOT = Path(__file__).resolve().parent.parent
BASELINE_DIR = ROOT / "specs"
REPORT = ROOT / "specs-diff.md"
MAX_DIFF_LINES = 400
TIMESTAMP = re.compile(r"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}")


def fetch(url: str) -> str:
    request = urllib.request.Request(url, headers={"User-Agent": "Platsbanken.NET-spec-drift"})
    with urllib.request.urlopen(request, timeout=60) as response:
        document = json.load(response)
    text = json.dumps(document, indent=2, sort_keys=True, ensure_ascii=False) + "\n"
    # Some specs embed a server-generated example timestamp in descriptions; mask it to avoid false drift.
    return TIMESTAMP.sub("<timestamp>", text)


def main() -> int:
    update = "--update" in sys.argv[1:]
    BASELINE_DIR.mkdir(exist_ok=True)
    sections = []

    for name, url in SPECS.items():
        current = fetch(url)
        path = BASELINE_DIR / f"{name}.json"

        if update:
            path.write_text(current, encoding="utf-8", newline="\n")
            print(f"updated {path.relative_to(ROOT)}")
            continue

        baseline = path.read_text(encoding="utf-8") if path.exists() else ""
        if baseline == current:
            print(f"ok      {name}")
            continue

        diff = list(
            difflib.unified_diff(
                baseline.splitlines(),
                current.splitlines(),
                fromfile=f"baseline/{name}.json",
                tofile=f"upstream/{name}.json",
                lineterm="",
            )
        )
        shown = diff[:MAX_DIFF_LINES]
        note = f"\n... {len(diff) - MAX_DIFF_LINES} more diff lines truncated" if len(diff) > MAX_DIFF_LINES else ""
        sections.append(f"### {name}\n\nSource: {url}\n\n```diff\n" + "\n".join(shown) + note + "\n```\n")
        print(f"DRIFT   {name} ({len(diff)} diff lines)")

    if update:
        return 0

    if sections:
        REPORT.write_text(
            "The upstream JobTech API specs changed. Review the diff, adapt the wire DTOs and mappers if needed, "
            "then run `python scripts/check_specs.py --update` and commit the new baselines.\n\n" + "\n".join(sections),
            encoding="utf-8",
            newline="\n",
        )
        return 1

    REPORT.unlink(missing_ok=True)
    return 0


if __name__ == "__main__":
    sys.exit(main())
