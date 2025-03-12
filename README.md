# gRPC Chat Anwendung

Dieses Repository enthält eine gRPC-basierte Chat-Anwendung mit einem Server und mehreren Clients. Die Anwendung ermöglicht eine bidirektionale, asynchrone Nachrichtenübertragung und unterstützt Broadcast-Nachrichten an alle verbundenen Clients.

## 📌 Features
- **gRPC-Server**:
  - Akzeptiert mehrere Client-Verbindungen gleichzeitig
  - Verarbeitet Nachrichten und sendet sie an alle verbundenen Clients
  - Speichert Chat-Protokolle in `chat_log.txt`
  - Unterstützt Benutzerstatus (Anmeldung und Abmeldung)

- **gRPC-Client (WPF-Anwendung)**:
  - Benutzer können sich mit einem Namen anmelden
  - Nachrichten werden mit **INFO, WARNING und ALERT** unterschieden und farblich markiert
  - Alle verbundenen Clients erhalten gesendete Nachrichten
  - Benutzer-Login/-Logout wird im Chat und im Log gespeichert

## 🛠️ Technologien
- **C#** mit **.NET 8.0**
- **gRPC (Google Remote Procedure Call)**
- **WPF für GUI-Clients**
- **Postman für API-Tests (manuell & automatisch)**
- **Google.Protobuf** für Nachrichtenstruktur
- **Grpc.AspNetCore** für Server-Implementierung

## 🚀 Installation & Setup
### 1️⃣ **Repository klonen**
```bash
 git clone https://github.com/username/GrpcChatApp.git
 cd GrpcChatApp
```

### 2️⃣ **Abhängigkeiten installieren**
```bash
 dotnet restore
```

### 3️⃣ **Server starten**
```bash
 cd GrpcChatServer
 dotnet run
```

### 4️⃣ **Client starten**
```bash
 cd ../GrpcChatClient
 dotnet run
```

## 📜 Protokoll (chat.proto)
```proto
syntax = "proto3";
option csharp_namespace = "GrpcChatServer.Protos";
package chat;

service ChatService {
  rpc SendMessage (stream ChatMessage) returns (stream ChatMessage);
  rpc UserStatusUpdate (UserStatus) returns (google.protobuf.Empty);
}

message UserStatus {
  string username = 1;
  bool isConnected = 2;
}

enum MessageType {
  INFO = 0;
  WARNING = 1;
  ALERT = 2;
}

message ChatMessage {
  string username = 1;
  string text = 2;
  MessageType messageType = 3;
}
```

## ✅ Tests & Ergebnisse
### **📝 Manuelle Tests mit Postman**
- `SendMessage` getestet mit INFO, WARNING, ALERT Nachrichten.
- `UserStatusUpdate` getestet für Benutzeranmeldung und -abmeldung.
- Logs in `chat_log.txt` überprüft.

### **🤖 Automatische Tests (Postman mit Assertions & Pre-/Post-Conditions)**
- Statuscodes geprüft (`Status code is 0`)
- Antwortzeit getestet (`Response time is below 200ms`)
- Header validiert (`Response metadata has "content-type"`)
- JSON-Format geprüft (`SendMessage` und `UserStatusUpdate` geben korrekte Werte zurück)

## 📂 Verzeichnisstruktur
```
GrpcChatApp/
│── GrpcChatServer/        # Server-Code
│── GrpcChatClient/        # Client-Code
│── AB03-PraxisAufgabeDoku.pdf  # Dokumentation
│── AB03-PraxisAufgabeVideo.mp4 # Demo-Video
│── README.md              # Diese Datei
```

## 🎥 Video-Demo
Ein Video zur Demonstration der Funktionen ist im Repository enthalten (`AB03-PraxisAufgabeVideo.mp4`).

## 📜 Lizenz
Dieses Projekt wurde im Rahmen der **AB03 Praxisaufgabe gRPC** erstellt und ist unter der **MIT-Lizenz** veröffentlicht.

🚀 Viel Spaß mit der gRPC Chat-Anwendung! 😊

