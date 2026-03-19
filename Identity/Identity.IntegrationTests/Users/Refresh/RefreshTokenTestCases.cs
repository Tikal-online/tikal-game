namespace Identity.IntegrationTests.Users.Refresh;

internal static class RefreshTokenTestCases
{
    public static IEnumerable<string> InvalidRefreshTokens =>
    [
        // empty token
        "",
        // invalid token
        "invalidRefreshToken",
        // wrong issuer
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiYWRtaW4iOnRydWUsIm5hbWUiOiJ1c2VybmFtZSIsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoyNTE2MjM5MDIyLCJpc3MiOiJodHRwczovL3dyb25nLnRpa2Fsb25saW5lLmNvbSIsImF1ZCI6Imh0dHBzOi8vdGlrYWxvbmxpbmUuY29tIn0.Zxg00gy2C1VhyQ_oHeB3eQQQBTTnyU1ONYhvWEzUyz0",
        // wrong audience
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiYWRtaW4iOnRydWUsIm5hbWUiOiJ1c2VybmFtZSIsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoyNTE2MjM5MDIyLCJpc3MiOiJodHRwczovL2F1dGgudGlrYWxvbmxpbmUuY29tIiwiYXVkIjoiaHR0cHM6Ly9ub3R0aGVjb3JyZWN0YXVkaWVuY2UuY29tIn0.KrIHstBVujT50ncGMkKJwvcwhEeJoJE4l42C9omCXRs",
        // no name claim
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoyNTE2MjM5MDIyLCJpc3MiOiJodHRwczovL2F1dGgudGlrYWxvbmxpbmUuY29tIiwiYXVkIjoiaHR0cHM6Ly90aWthbG9ubGluZS5jb20ifQ.EBKGD1r8lhO86MqVdSXbAn0swWa8_yRRM_3veWK4GIE",
        // expired
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiYWRtaW4iOnRydWUsIm5hbWUiOiJ1c2VybmFtZSIsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoxNTE2MjM5MDIyLCJpc3MiOiJodHRwczovL2F1dGgudGlrYWxvbmxpbmUuY29tIiwiYXVkIjoiaHR0cHM6Ly90aWthbG9ubGluZS5jb20ifQ.68PO_kgLLAKtuPDSuJR-17zsD6vUao_wlB_4tWIWB7Q"
    ];
}