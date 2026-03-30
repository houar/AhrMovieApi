let port = pm.globals.get("DOMT-RESTAPI-PORTTOK");
const tokenUrl = `https://localhost:${port}/api/token`;

const getTokenRequest = {
    url: tokenUrl,
    method: 'POST',
    header: {
        'Content-Type': 'application/json'
    },
    body: {
        mode: 'raw',
        raw: JSON.stringify({
            userid: 'd8566de3-b1a6-4a9b-b842-8e3887a82e41',
            email: 'madjid@houar.eu',
            customClaims: {
                 admin: true,
                 trusted_member: true
            }
        })
    }
};

pm.sendRequest(getTokenRequest, function (err, res) {
    if (err) {
        console.error('Token request failed:', err);
    } else {
        pm.globals.set("DOMT-RESTAPI-AUTH-TOKEN", res.text());
    }
});