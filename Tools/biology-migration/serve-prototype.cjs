// Local review only. Exposes the prototype and three fonts, no other project files.
const http = require('node:http');
const fs = require('node:fs');
const path = require('node:path');
const root = path.resolve(__dirname, '../..');
const files = new Map();
for (const name of ['index.html', 'layout.html', 'app.js', 'style.css', 'anatomy-layout.html', 'anatomy-layout.css', 'anatomy-layout.js']) {
  files.set('/' + name, path.join(root, 'docs/prototypes/biologia', name));
}
files.set('/', files.get('/index.html'));
for (const name of ['Montserrat-Regular.ttf', 'Montserrat-Bold.ttf', 'Montserrat-ExtraBold.ttf']) {
  const key = '/Assets/_Project/UI/Fonts/Montserrat/' + name;
  files.set(key, path.join(root, key.slice(1)));
}
const types = { '.html': 'text/html; charset=utf-8', '.js': 'text/javascript; charset=utf-8', '.css': 'text/css; charset=utf-8', '.ttf': 'font/ttf' };
const server = http.createServer((req, res) => {
  if (req.method !== 'GET') { res.writeHead(405); return res.end(); }
  const file = files.get(new URL(req.url, 'http://localhost').pathname);
  if (!file) { res.writeHead(404); return res.end(); }
  res.writeHead(200, { 'Content-Type': types[path.extname(file)], 'Cache-Control': 'no-store' });
  fs.createReadStream(file).pipe(res);
});
server.listen(0, '127.0.0.1', () => {
  const url = 'http://127.0.0.1:' + server.address().port;
  fs.mkdirSync(path.join(root, 'tmp/biology-mcp'), { recursive: true });
  fs.writeFileSync(path.join(root, 'tmp/biology-mcp/prototype-server.json'), JSON.stringify({ url, pid: process.pid }));
  console.log(url);
});
