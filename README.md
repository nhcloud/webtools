# Developer Tools
Free, fast, no-signup web tools for developers. Built with ASP.NET Core (.NET 10 / C# 13) and Bootstrap 5. No data is persisted server-side.

## Features
- **FCM Push Notification Tester**: Send test Firebase Cloud Messaging notifications using HTTP v1 API with Google Service Account JSON.
- **QR Code Generator**: Create downloadable QR codes for any URL.
- **OAuth Access Token Generator & JWT Decoder**:
  - Generate Azure AD (Microsoft Entra ID) access tokens using client credentials flow
  - Built-in JWT decoder with jwt.ms-style interface
  - Color-coded token parts (header/payload/signature) with full background highlighting
  - Syntax-highlighted decoded JSON with claims table
  - Tabs for "Decoded Token" and "Claims" views
  - Token expiration validation and issuer detection
  - Paste any JWT to decode - no generation required
- **JSON Formatter & Validator**: Pretty-print, minify, and validate JSON with color highlighting (runs in browser).
- **Miscellaneous Tools**:
  - UNIX Time converter (UTC/local)
  - URL Encode / Decode
  - Base64 Encode / Decode

## Tech Stack
- ASP.NET Core 10 / C# 13
- Bootstrap 5, Font Awesome
- Google.Apis.Auth (for FCM HTTP v1)
- Minimal server logic; client-side processing where possible

## Run Locally
```bash
dotnet run --project src/ToolsWebsite.csproj
# then open https://localhost:7223 or http://localhost:5230
```

## Security / Privacy
- No data stored
- HTTPS enforced
- Inputs processed in-memory / client-side when feasible
- JWT decoding happens entirely in browser (token never leaves your machine)

## Contributing
PRs welcome. Open an issue for ideas or bugs.

## Credits
Assisted by AI agent modes: Sonnet-4 and GPT-5 for refinement and content generation support.

## License
MIT (see repository LICENSE if present).

---
Built by Udaiappa Ramachandran (Udai) • https://udai.io
