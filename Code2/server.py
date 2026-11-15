from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
import json

state = {"lyrics": "", "mode": "normal"}

class Handler(SimpleHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/sync":
            data = json.dumps(state).encode()
            self.send_response(200)
            self.send_header("Content-Type", "application/json")
            self.send_header("Cache-Control", "no-store")
            self.end_headers()
            self.wfile.write(data)
        else:
            super().do_GET()

    def do_POST(self):
        if self.path != "/sync":
            self.send_error(404)
            return

        try:
            length = int(self.headers.get("Content-Length", 0))
            if length > 100000:
                self.send_error(413)
                return

            data = json.loads(self.rfile.read(length))
            state["lyrics"] = str(data.get("lyrics", ""))
            state["mode"] = str(data.get("mode", "normal"))

            self.send_response(204)
            self.end_headers()
        except Exception:
            self.send_error(400)

ThreadingHTTPServer(("0.0.0.0", 8080), Handler).serve_forever()
