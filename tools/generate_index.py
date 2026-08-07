#!/usr/bin/env python3
"""Genera SNIPPETS.md, el catalogo completo de la coleccion.

El indice se construye leyendo los propios archivos .snippet, de modo que no
haya que mantener a mano ni el conteo ni las descripciones.

Uso:
    python3 tools/generate_index.py          # reescribe SNIPPETS.md
    python3 tools/generate_index.py --check   # falla si esta desactualizado
"""

import argparse
import os
import sys
import xml.etree.ElementTree as ET

NS = {"s": "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet"}

HEADER = """<!-- Archivo generado por tools/generate_index.py. No editar a mano. -->

# Catalogo de snippets

{total} snippets en {categories} categorias. Escribe el shortcut en un archivo
`.cs` y presiona `Tab` dos veces para expandirlo.

"""


def read_snippet(path):
    node = ET.parse(path).getroot()

    def text(tag):
        element = node.find(".//s:%s" % tag, NS)
        return element.text.strip() if element is not None and element.text else ""

    literals = []
    for literal in node.findall(".//s:Literal", NS):
        literal_id = literal.find("s:ID", NS)
        if literal_id is not None and literal_id.text:
            literals.append(literal_id.text.strip())

    return {
        "shortcut": text("Shortcut"),
        "title": text("Title"),
        "description": text("Description"),
        "literals": literals,
    }


def build_index(root):
    snippets_dir = os.path.join(root, "snippets")
    categories = []

    for category in sorted(os.listdir(snippets_dir)):
        category_path = os.path.join(snippets_dir, category)
        if not os.path.isdir(category_path):
            continue

        entries = []
        for filename in sorted(os.listdir(category_path)):
            if filename.endswith(".snippet"):
                entries.append(read_snippet(os.path.join(category_path, filename)))

        if entries:
            categories.append((category, entries))

    total = sum(len(entries) for _name, entries in categories)

    parts = [HEADER.format(total=total, categories=len(categories))]

    for name, entries in categories:
        parts.append("## `%s`\n\n" % name)
        parts.append("| Shortcut | Titulo | Descripcion | Campos editables |\n")
        parts.append("|---|---|---|---|\n")

        for entry in entries:
            literals = ", ".join("`%s`" % item for item in entry["literals"]) or "—"
            parts.append(
                "| `%s` | %s | %s | %s |\n"
                % (
                    entry["shortcut"],
                    entry["title"],
                    entry["description"].replace("|", "\\|"),
                    literals,
                )
            )

        parts.append("\n")

    return "".join(parts)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        default=os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
        help="Raiz del repositorio (por defecto, la que contiene tools/)",
    )
    parser.add_argument(
        "--check",
        action="store_true",
        help="No escribe: falla si SNIPPETS.md difiere del contenido generado",
    )
    args = parser.parse_args()

    content = build_index(args.root)
    target = os.path.join(args.root, "SNIPPETS.md")

    if args.check:
        if not os.path.exists(target):
            print("ERROR: falta SNIPPETS.md. Ejecuta: python3 tools/generate_index.py")
            return 1

        with open(target, "r", encoding="utf-8") as handle:
            current = handle.read()

        if current != content:
            print(
                "ERROR: SNIPPETS.md esta desactualizado. "
                "Ejecuta: python3 tools/generate_index.py"
            )
            return 1

        print("SNIPPETS.md esta al dia.")
        return 0

    with open(target, "w", encoding="utf-8") as handle:
        handle.write(content)

    print("SNIPPETS.md regenerado.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
