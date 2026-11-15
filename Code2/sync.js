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

            // Display lyrics directly without using localStorage
if (data.mode === "scrolling") {
    lyrics.style.display = "none";
    scrollingLyrics.style.display = "block";
} else {
    scrollingLyrics.style.display = "none";
    lyrics.style.display = "flex";
}

if (data.lyrics) {
    if (lyrics.textContent !== data.lyrics) {
        lyrics.textContent = data.lyrics;
        fitLyrics();
    }
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
