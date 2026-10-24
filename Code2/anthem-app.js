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