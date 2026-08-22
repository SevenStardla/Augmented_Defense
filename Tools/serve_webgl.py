import argparse
import functools
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer


class WebGLRequestHandler(SimpleHTTPRequestHandler):
    def guess_type(self, path):
        if path.endswith(".js.gz"):
            return "application/javascript"
        if path.endswith(".wasm.gz"):
            return "application/wasm"
        if path.endswith(".data.gz"):
            return "application/octet-stream"
        return super().guess_type(path)

    def end_headers(self):
        if self.path.split("?", 1)[0].endswith(".gz"):
            self.send_header("Content-Encoding", "gzip")
        self.send_header("Cache-Control", "no-store")
        super().end_headers()


def main():
    parser = argparse.ArgumentParser(description="Serve a Unity WebGL build with gzip headers.")
    parser.add_argument("--directory", default="Builds/WebGL")
    parser.add_argument("--port", type=int, default=8000)
    args = parser.parse_args()

    handler = functools.partial(WebGLRequestHandler, directory=args.directory)
    server = ThreadingHTTPServer(("127.0.0.1", args.port), handler)
    print(f"Serving {args.directory} at http://127.0.0.1:{args.port}")
    server.serve_forever()


if __name__ == "__main__":
    main()
