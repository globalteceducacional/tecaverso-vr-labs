"""Preparation only: restore curated photos or refresh evidence; never called by Unity runtime."""
import argparse
import json
import pathlib
import time
import urllib.request

ROOT = pathlib.Path(__file__).resolve().parents[2]
DATA = ROOT / 'Assets/_Project/Laboratories/Chemistry/PeriodicBonds/Data'
CACHE = ROOT / 'Temp/periodic-import'
CACHE.mkdir(parents=True, exist_ok=True)
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--photos', action='store_true', help='Restore missing photos from the reviewed local provenance manifest')
parser.add_argument('--evidence', action='store_true', help='Cache PubChem evidence for editorial review')
args = parser.parse_args()


def download(url):
    req = urllib.request.Request(url, headers={'User-Agent': 'TecaversoEducationalOfflineCatalog/1.0'})
    with urllib.request.urlopen(req, timeout=40) as response:
        return response.read()


if args.photos:
    for photo in json.loads((DATA / 'PhotoProvenance.json').read_text(encoding='utf8')):
        target = DATA / 'Resources/PeriodicSamples' / photo['file']
        if target.exists():
            continue
        # Existing photos and their Unity GUIDs are never overwritten by this command.
        payload = download(photo['download'])
        if not (payload.startswith(b'\xff\xd8') or payload.startswith(b'\x89PNG')):
            raise ValueError(f'Not a supported photo: {photo["number"]}')
        target.parent.mkdir(parents=True, exist_ok=True)
        temporary = CACHE / 'photo-download.tmp'
        temporary.write_bytes(payload)
        temporary.replace(target)
        print('Restored', target.name, flush=True)
        time.sleep(1)

if args.evidence:
    evidence = []
    for number in range(1, 119):
        path = CACHE / f'pubchem-{number}.json'
        if not path.exists():
            payload = download(f'https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/element/{number}/JSON')
            json.loads(payload)  # Validate before saving a usable cache entry.
            path.write_bytes(payload)
            time.sleep(1)
        record = json.loads(path.read_text(encoding='utf8'))['Record']
        evidence.append({'number': number, 'source': f'https://pubchem.ncbi.nlm.nih.gov/element/{number}',
                         'references': record.get('Reference', [])})
    (CACHE / 'evidence.json').write_text(json.dumps(evidence, ensure_ascii=False, indent=2), encoding='utf8')

if not args.photos and not args.evidence:
    parser.print_help()
