#!/usr/bin/env bash
# Dev environment setup without Docker: ACA sandbox, Codespaces, or plain Linux/macOS/WSL.
# Installs the same MkDocs toolchain as the Dockerfile into a Python venv. Idempotent.
#
#   .sandbox/setup.sh
#   . .venv/bin/activate && mkdocs serve --dev-addr 0.0.0.0:8000
#
# In an ACA sandbox, the venv lives on the persistent volume ($SANDBOX_STATE_DIR/venv)
# and .venv is a symlink to it, so it survives sandbox rebuilds.
set -euo pipefail
cd "$(dirname "$0")/.."

VENV=".venv"
if [ -n "${SANDBOX_STATE_DIR:-}" ]; then
  VENV="$SANDBOX_STATE_DIR/venv"
  [ -e .venv ] || ln -s "$VENV" .venv
fi

# Single source of truth for the mkdocs-material version: the Dockerfile base image tag.
MATERIAL="$(sed -n 's#^FROM squidfunk/mkdocs-material:\([^ @]*\).*#\1#p' Dockerfile)"
[ -n "$MATERIAL" ] || { echo "cannot read the mkdocs-material version from Dockerfile" >&2; exit 1; }

[ -x "$VENV/bin/python" ] || python3 -m venv "$VENV"
"$VENV/bin/pip" install -q --upgrade pip
"$VENV/bin/pip" install -q "mkdocs-material==$MATERIAL" -r requirements.txt

echo "ready (mkdocs-material $MATERIAL): . .venv/bin/activate && mkdocs serve --dev-addr 0.0.0.0:8000"
