#!/usr/bin/env python3
"""Render docs/architecture/architecture.json as a standalone HTML dashboard.

The arch-wiki Skill owns code scanning/comparison. This script is intentionally
presentation-only: it renders the manifest, drift/conflicts and C4 views.
"""

from __future__ import annotations

import argparse
import copy
import html
import json
from pathlib import Path
from typing import Any


def esc(value: Any) -> str:
    return html.escape(str(value if value is not None else ""))


def graph(view: dict[str, Any]) -> str:
    lines = ["flowchart LR"]
    for node in view.get("nodes", []):
        node_id = str(node.get("id", "node")).replace("-", "_")
        label = str(node.get("label", node_id)).replace('"', "'")
        lines.append(f'  {node_id}["{label}"]')
    for edge in view.get("edges", []):
        source = str(edge.get("from", "")).replace("-", "_")
        target = str(edge.get("to", "")).replace("-", "_")
        label = str(edge.get("label", "")).replace('"', "'")
        lines.append(f'  {source} -->|"{label}"| {target}' if label else f"  {source} --> {target}")
    return "\n".join(lines)


def demo_conflict() -> dict[str, Any]:
    return {
        "id": "demo-cross-module-dependency",
        "status": "CONFLICT",
        "severity": "HIGH",
        "category": "Cross-module dependency",
        "c4Level": "C3",
        "module": "WorkItems",
        "component": "Application",
        "expected": "WorkItems.Application\n    ↓\nProjects.Contracts",
        "observed": "WorkItems.Application\n    ↓\nProjects.Infrastructure",
        "codeLocations": [{
            "path": "src/Modules/WorkItems/.../CreateWorkItemHandler.cs",
            "line": 42,
            "evidence": "Direct dependency on Projects.Infrastructure"
        }],
        "docLocations": [
            {"path": "docs/architecture/architecture.json", "pointer": "$.c4.components"},
            {"path": "docs/decisions/0002-cross-module-contracts.md", "section": "Decision"}
        ],
        "impact": "Direct dependency bypasses the published module contract and increases coupling/change surface.",
        "suggestedActions": [
            "Change code to consume Projects.Contracts.",
            "Or review/update the architecture decision if intentional."
        ],
        "demo": True
    }


def conflict_card(item: dict[str, Any]) -> str:
    severity = str(item.get("severity", "LOW")).upper()
    code = "".join(
        f"<li><code>{esc(x.get('path'))}{':' + esc(x.get('line')) if x.get('line') else ''}</code>"
        f"{' — ' + esc(x.get('evidence')) if x.get('evidence') else ''}</li>"
        for x in item.get("codeLocations", [])
    ) or "<li>Not specified</li>"
    docs = "".join(
        f"<li><code>{esc(x.get('path'))}</code>"
        f"{' — ' + esc(x.get('section') or x.get('pointer')) if x.get('section') or x.get('pointer') else ''}</li>"
        for x in item.get("docLocations", [])
    ) or "<li>Not specified</li>"
    actions = "".join(f"<li>{esc(x)}</li>" for x in item.get("suggestedActions", []))
    demo = '<span class="demo">DEMO CONFLICT</span>' if item.get("demo") else ""
    return f"""
<article class="conflict {severity.lower()}">
  <div class="head"><div><span class="severity">{esc(severity)}</span>{demo}<strong>{esc(item.get('category','Architecture conflict'))}</strong></div><span class="pill">{esc(item.get('c4Level','C3'))}</span></div>
  <p class="muted">{esc(item.get('module',''))}{' / ' + esc(item.get('component')) if item.get('component') else ''}</p>
  <div class="diff">
    <section><h4 class="good">EXPECTED</h4><pre>{esc(item.get('expected',''))}</pre></section>
    <section><h4 class="bad">OBSERVED IN CODE</h4><pre>{esc(item.get('observed',''))}</pre></section>
  </div>
  <div class="detail">
    <section><h4>Code location</h4><ul>{code}</ul></section>
    <section><h4>Documentation</h4><ul>{docs}</ul></section>
    <section><h4>Why / Impact</h4><p>{esc(item.get('impact','Not specified'))}</p></section>
    <section><h4>Suggested resolution</h4><ol>{actions or '<li>Review the difference.</li>'}</ol></section>
  </div>
</article>"""


def render(data: dict[str, Any], demo: bool) -> str:
    data = copy.deepcopy(data)
    drift = data.setdefault("drift", {})
    summary = drift.setdefault("summary", {})
    conflicts = drift.setdefault("conflicts", [])
    if demo:
        conflicts.append(demo_conflict())
        summary["conflicts"] = int(summary.get("conflicts", 0)) + 1
        summary["high"] = int(summary.get("high", 0)) + 1
        drift["overallStatus"] = "DRIFT DETECTED"

    meta = data.get("meta", {})
    c4 = data.get("c4", {})
    cards = "".join(conflict_card(x) for x in conflicts) or '<div class="empty">No unresolved architecture conflicts in the current manifest.</div>'

    def metric(label: str, key: str) -> str:
        return f'<div class="metric"><span>{esc(label)}</span><strong>{esc(summary.get(key,0))}</strong></div>'

    c4_html = []
    for key, title in (("context", "C1 — System Context"), ("containers", "C2 — Containers")):
        view = c4.get(key, {})
        if view:
            c4_html.append(f'<section class="panel"><h3>{title}</h3><p class="muted">{esc(view.get("description",""))}</p><pre class="mermaid">{graph(view)}</pre></section>')
    for view in c4.get("components", []):
        c4_html.append(f'<section class="panel"><h3>C3 — {esc(view.get("scope","Components"))}</h3><p class="muted">{esc(view.get("description",""))}</p><pre class="mermaid">{graph(view)}</pre></section>')

    modules = "".join(
        f'<tr><td>{esc(m.get("name"))}</td><td><code>{esc(m.get("basePath","—"))}</code></td><td>{len(m.get("endpoints",[]))}</td><td>{esc(m.get("description",""))}</td></tr>'
        for m in data.get("modules", [])
    )

    return f'''<!doctype html>
<html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>{esc(meta.get("title","Architecture Dashboard"))}</title>
<script type="module">import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.esm.min.mjs';mermaid.initialize({{startOnLoad:true,theme:'dark'}});</script>
<style>
:root{{--bg:#0b1020;--panel:#111831;--line:#263253;--muted:#93a4c7;--good:#25c26e;--bad:#ff5d69;--warn:#f3b33d}}*{{box-sizing:border-box}}body{{margin:0;background:var(--bg);color:#eef3ff;font:14px/1.55 Segoe UI,Arial,sans-serif}}header{{padding:20px 28px;border-bottom:1px solid var(--line);position:sticky;top:0;background:#0d1428f2;z-index:2}}header h1{{margin:0}}header p,.muted{{color:var(--muted)}}.layout{{display:grid;grid-template-columns:220px 1fr}}nav{{padding:22px 16px;border-right:1px solid var(--line);height:calc(100vh - 83px);position:sticky;top:83px}}nav a{{display:block;color:#cad6f4;text-decoration:none;padding:9px;border-radius:8px}}nav a:hover{{background:#17213e}}main{{padding:28px;max-width:1400px;width:100%}}.health{{display:grid;grid-template-columns:1.5fr repeat(4,1fr);gap:12px;margin-bottom:12px}}.status,.metric,.panel,.conflict,.empty{{background:var(--panel);border:1px solid var(--line);border-radius:14px}}.status,.metric{{padding:18px}}.status strong,.metric strong{{display:block;font-size:24px}}.metric span{{color:var(--muted)}}.conflict{{padding:20px;margin:14px 0;border-left:4px solid var(--bad)}}.conflict.medium{{border-left-color:var(--warn)}}.head{{display:flex;justify-content:space-between;align-items:center;gap:12px}}.severity,.pill,.demo{{padding:3px 8px;border-radius:999px;font-size:11px;font-weight:700;margin-right:7px}}.severity{{background:#67202a}}.pill{{background:#183b6a}}.demo{{background:#5a4310;color:#ffe6a0}}.diff,.detail{{display:grid;grid-template-columns:1fr 1fr;gap:12px}}.diff section,.detail section{{background:#0d1428;border:1px solid var(--line);border-radius:10px;padding:14px}}.good{{color:var(--good)}}.bad{{color:var(--bad)}}pre{{white-space:pre-wrap;overflow:auto}}.panel{{padding:20px;margin:14px 0}}.empty{{padding:20px;color:var(--muted)}}table{{width:100%;border-collapse:collapse}}th,td{{padding:10px;border-bottom:1px solid var(--line);text-align:left}}th{{color:var(--muted)}}@media(max-width:900px){{.layout{{grid-template-columns:1fr}}nav{{display:none}}.health{{grid-template-columns:1fr 1fr}}.diff,.detail{{grid-template-columns:1fr}}}}
</style></head><body>
<header><h1>{esc(meta.get("title","Architecture Dashboard"))}</h1><p>Version {esc(meta.get("version","—"))} · Generated {esc(meta.get("generatedAt","—"))} · Observed {esc(drift.get("observedAt","—"))}</p></header>
<div class="layout"><nav><strong>NAVIGATION</strong><a href="#health">Architecture Health</a><a href="#drift">Architecture Drift</a><a href="#c4">C4 Model</a><a href="#modules">Modules</a></nav><main>
<section id="health"><h2>Architecture Health</h2><div class="health"><div class="status"><span>Overall Status</span><strong>{esc(drift.get("overallStatus","UNKNOWN"))}</strong></div>{metric("Matches","matches")}{metric("Conflicts","conflicts")}{metric("Code Only","codeOnly")}{metric("Docs Only","docsOnly")}</div><div class="health">{metric("High","high")}{metric("Medium","medium")}{metric("Low","low")}</div></section>
<section id="drift"><h2>Architecture Drift</h2>{cards}</section>
<section id="c4"><h2>C4 Model</h2>{''.join(c4_html)}<p class="muted">C4 Code (level 4) is intentionally generated only on demand for a focused component.</p></section>
<section id="modules" class="panel"><h2>Modules</h2><table><thead><tr><th>Module</th><th>Base path</th><th>Endpoints</th><th>Description</th></tr></thead><tbody>{modules}</tbody></table></section>
<p class="muted">Code is observed reality. Conflict evidence is captured before the living manifest is synchronized.</p>
</main></div></body></html>'''


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--manifest", default=None)
    parser.add_argument("--output", default=None)
    parser.add_argument("--demo-conflict", action="store_true")
    args = parser.parse_args()

    script_dir = Path(__file__).resolve().parent
    manifest = Path(args.manifest) if args.manifest else script_dir / "architecture.json"
    output = Path(args.output) if args.output else script_dir / "architecture.html"
    if not manifest.exists():
        raise SystemExit(f"Architecture manifest not found: {manifest}")
    data = json.loads(manifest.read_text(encoding="utf-8"))
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(render(data, args.demo_conflict), encoding="utf-8")
    print(f"Generated {output}")


if __name__ == "__main__":
    main()
