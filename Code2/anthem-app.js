const anthemSearch = document.getElementById("anthemSearch");
const anthemInfo = document.getElementById("anthemInfo");
const anthemResults = document.getElementById("anthemResults");


// SEARCH FOR FOLDER

anthemSearch.addEventListener("input", searchAnthemFolders);

function searchAnthemFolders() {

    const query = anthemSearch.value.trim().toLowerCase();

    anthemResults.replaceChildren();

    if (!query) {
        anthemInfo.textContent =
            "Enter a hymn number or type anthem.";
        return;
    }

    const matches = anthemFolders.filter(folder =>
        folder.name.trim().toLowerCase() === query
    );

    if (matches.length === 0) {
        anthemInfo.textContent = "No matching folder found.";
        return;
    }

    anthemInfo.textContent =
        `${matches.length} folder found.`;

    matches.forEach(folder => {

        const folderContainer = document.createElement("div");

        folderContainer.style.margin = "12px 0";
        folderContainer.style.border = "1px solid #555";
        folderContainer.style.borderRadius = "8px";
        folderContainer.style.overflow = "hidden";


        // FOLDER BUTTON

        const folderButton = document.createElement("button");

folderButton.textContent =
    folder.name === "anthem"
        ? "Choir Anthem"
        : folder.name === "sunday school"
            ? `Sunday School - ${folder.language || ""}`
            : `Hymn ${folder.name} - ${folder.language || ""}`;
            
        folderButton.style.width = "100%";
        folderButton.style.padding = "14px";
        folderButton.style.cursor = "pointer";


        // FOLDER CONTENT

        const folderContent = document.createElement("div");

        folderContent.style.display = "none";
        folderContent.style.padding = "12px";


        folderButton.addEventListener("click", () => {

            const isOpen =
                folderContent.style.display === "block";

            folderContent.style.display =
                isOpen ? "none" : "block";

        });


        // CREATE VERSES

        folder.items.forEach(item => {

            const verseContainer = document.createElement("div");

            verseContainer.style.padding = "15px";
            verseContainer.style.margin = "10px 0";
            verseContainer.style.border = "1px solid #444";
            verseContainer.style.borderRadius = "8px";


            const verseTitle = document.createElement("h3");

            verseTitle.textContent = item.title;


            const verseLyrics = document.createElement("p");

            verseLyrics.textContent = item.lyrics;
            verseLyrics.style.whiteSpace = "pre-line";
            verseLyrics.style.lineHeight = "1.6";


            const displayButton = document.createElement("button");

            displayButton.textContent = "DISPLAY";

            displayButton.style.padding = "10px 20px";
            displayButton.style.cursor = "pointer";


            // SEND LYRICS TO DISPLAY 2

            displayButton.addEventListener("click", () => {

                const displayTitle =
                    folder.name === "anthem"
                        ? `Choir Anthem - ${item.title}`
                        : `Hymn ${folder.name} - ${item.title}`;

                localStorage.setItem(
                    "churchDisplayTitle",
                    displayTitle
                );

                localStorage.setItem(
                    "churchDisplayLyrics",
                    item.lyrics
                );

                displayButton.textContent = "DISPLAYED";

                setTimeout(() => {
                    displayButton.textContent = "DISPLAY";
                }, 1000);

            });


            verseContainer.append(
                verseTitle,
                verseLyrics,
                displayButton
            );

            folderContent.appendChild(verseContainer);

        });


        folderContainer.append(
            folderButton,
            folderContent
        );

        anthemResults.appendChild(folderContainer);

    });

}

// ========================================
// FREQUENTLY USED SONGS SIDEBAR
// ========================================

const frequentToggle = document.getElementById("frequentToggle");
const frequentSidebar = document.getElementById("frequentSidebar");
const frequentClose = document.getElementById("frequentClose");
const frequentList = document.getElementById("frequentList");

// Open sidebar
frequentToggle.addEventListener("click", () => {
    frequentSidebar.classList.add("open");
});

// Close sidebar
frequentClose.addEventListener("click", () => {
    frequentSidebar.classList.remove("open");
});

// Display frequently used songs
function loadFrequentAnthems() {
    frequentList.replaceChildren();

    if (frequentAnthems.length === 0) {
        const message = document.createElement("p");
        message.textContent = "No frequently used songs added yet.";
        frequentList.appendChild(message);
        return;
    }

    frequentAnthems.forEach(folder => {

        // Song container
        const songContainer = document.createElement("div");
        songContainer.className = "frequentSong";

        // Song title
        const songButton = document.createElement("button");
        songButton.className = "frequentSongButton";

        const isNumberedHymn = /^\d+$/.test(String(folder.name).trim());

        const songName = isNumberedHymn
            ? `Hymn ${folder.name}`
            : folder.name;

        songButton.textContent = folder.language
            ? `${songName} - ${folder.language}`
            : songName;

        // Verses container
        const versesContainer = document.createElement("div");
        versesContainer.style.display = "none";

        // Expand or collapse song
        songButton.addEventListener("click", () => {
            const isOpen = versesContainer.style.display === "block";

            versesContainer.style.display = isOpen
                ? "none"
                : "block";
        });

        // Add verses
        folder.items.forEach(item => {

            const verseContainer = document.createElement("div");
            verseContainer.className = "frequentVerse";

            const verseTitle = document.createElement("h3");
            verseTitle.textContent = item.title;

            const verseLyrics = document.createElement("p");
            verseLyrics.textContent = item.lyrics;

            // DISPLAY button
            const displayButton = document.createElement("button");
            displayButton.className = "frequentDisplay";
            displayButton.textContent = "DISPLAY";

            displayButton.addEventListener("click", () => {

                // Send lyrics to the existing projection screen
                const displayTitle = `${songName} - ${item.title}`;

                localStorage.setItem(
                    "churchDisplayTitle",
                    displayTitle
                );

                localStorage.setItem(
                    "churchDisplayLyrics",
                    item.lyrics
                );

                displayButton.textContent = "DISPLAYED";

                setTimeout(() => {
                    displayButton.textContent = "DISPLAY";
                }, 1000);

                // Sidebar remains open
            });

            verseContainer.append(
                verseTitle,
                verseLyrics,
                displayButton
            );

            versesContainer.appendChild(verseContainer);
        });

        songContainer.append(
            songButton,
            versesContainer
        );

        frequentList.appendChild(songContainer);
    });
}

// Load songs when the page opens
loadFrequentAnthems();
