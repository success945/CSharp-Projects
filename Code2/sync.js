const isProjection = location.pathname.endsWith("display.html");

if (isProjection) {
    // TV or second phone: receive lyrics
    async function receiveLyrics() {
        try {
            const response = await fetch("/sync", {
                cache: "no-store"
            });

            if (!response.ok) return;

            const data = await response.json();

            if (data.lyrics !==
                localStorage.getItem("churchDisplayLyrics")) {
                localStorage.setItem(
                    "churchDisplayLyrics",
                    data.lyrics
                );
                updateDisplay();
            }

            if (data.mode !==
                localStorage.getItem("churchDisplayMode")) {
                localStorage.setItem(
                    "churchDisplayMode",
                    data.mode
                );
                applyDisplayMode();
            }
        } catch (error) {
            console.log("Waiting for connection");
        }
    }

    receiveLyrics();
    setInterval(receiveLyrics, 500);

} else {
    // Redmi: send selected lyrics
    let previousState = "";

    async function sendLyrics() {
        const data = {
            lyrics: localStorage.getItem("churchDisplayLyrics") || "",
            mode: localStorage.getItem("churchDisplayMode") || "normal"
        };

        const currentState = JSON.stringify(data);

        if (currentState === previousState) return;

        try {
            const response = await fetch("/sync", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: currentState
            });

            if (response.ok) {
                previousState = currentState;
            }
        } catch (error) {
            console.log("Connection unavailable");
        }
    }

    sendLyrics();
    setInterval(sendLyrics, 300);
}
