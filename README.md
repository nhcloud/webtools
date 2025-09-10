# Developer Tools (Udai)

Free, fast, no?signup web tools for developers. Built with ASP.NET Core (.NET 9 / C# 13) and Bootstrap 5. No data is persisted server?side.

## Features
- FCM Push Notification Tester: Send test Firebase Cloud Messaging notifications using server key + device token.
- QR Code Generator: Create downloadable QR codes for any URL.
- OAuth Access Token Generator: Client credentials flow for Azure AD (Microsoft Entra ID) apps.
- JSON Formatter & Validator: Pretty?print, minify, and validate JSON with color highlighting (runs in browser).
- Miscellaneous Tools:
  - UNIX Time converter (UTC/local)
  - URL Encode / Decode
  - Base64 Encode / Decode

## Tech Stack
- ASP.NET Core 9 / C# 13
- Bootstrap 5, Font Awesome
- Minimal server logic; client-side processing where possible

## Run Locally
```bash
dotnet run --project src/ToolsWebsite.csproj
# then open https://localhost:5001 or http://localhost:5000
```

## Security / Privacy
- No data stored
- HTTPS enforced
- Inputs processed in-memory / client-side when feasible

## Contributing
PRs welcome. Open an issue for ideas or bugs.

## License
MIT (see repository LICENSE if present).

---
Built by Udaiappa Ramachandran (Udai) • https://udai.io