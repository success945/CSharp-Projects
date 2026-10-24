const searchBox =
    document.getElementById("searchBox");

const resultsBox =
    document.getElementById("results");

const searchInfo =
    document.getElementById("searchInfo");

const overlay =
    document.getElementById("overlay");

const popupTitle =
    document.getElementById("popupTitle");

const popupLyrics =
    document.getElementById("popupLyrics");

const displayButton =
    document.getElementById("displayButton");

const cancelButton =
    document.getElementById("cancelButton");


let selectedSong = null;


/*
========================================
SETTINGS
========================================
*/

const OPENING_LINES = 3;
const MAX_RESULTS = 10;


/*
========================================
SEARCH WHILE TYPING
========================================
*/

searchBox.addEventListener(
    "input",
    function () {

        const query =
            searchBox.value.trim();

        if (!query) {

            resultsBox.innerHTML = "";

            searchInfo.textContent =
                "Start typing to find a song.";

            return;
        }

        const results =
            searchSongs(query);

        showResults(results);
    }
);


/*
========================================
MAIN SEARCH
========================================
*/

function searchSongs(query) {

    const queryWords =
        tokenize(query);

    if (queryWords.length === 0)
        return [];

    const results = [];

    for (const song of songs) {

        const lyricWords =
            tokenizeWithPositions(
                song.title + " " + song.lyrics
            );

        const result =
            calculateSongScore(
                queryWords,
                lyricWords,
                song
            );

        if (result.score > 0) {
            results.push(result);
        }
    }

    results.sort((a, b) => {

        if (
            Math.abs(
                b.score - a.score
            ) > 2
        ) {
            return b.score - a.score;
        }

        return (
            a.bestPosition -
            b.bestPosition
        );
    });

    return results.slice(
        0,
        MAX_RESULTS
    );
}

/*
========================================
SCORING
========================================
*/

function calculateSongScore(
    queryWords,
    lyricWords,
    song
) {

    if (lyricWords.length === 0) {

        return {
            song,
            score: 0,
            bestPosition: Infinity,
            bestMatchText: ""
        };
    }

    let totalSimilarity = 0;
    let matchedWords = 0;
    let bestPosition = Infinity;

    const matchedPositions = [];

    for (const queryWord of queryWords) {

        let bestSimilarity = 0;
        let bestIndex = -1;

        for (
            let i = 0;
            i < lyricWords.length;
            i++
        ) {

            const similarity =
                wordSimilarity(
                    queryWord,
                    lyricWords[i].word
                );

            if (
                similarity >
                bestSimilarity
            ) {

                bestSimilarity =
                    similarity;

                bestIndex = i;
            }
        }

        if (
            bestIndex !== -1 &&
            bestSimilarity >= 0.58
        ) {

            matchedWords++;

            totalSimilarity +=
                bestSimilarity;

            matchedPositions.push(
                bestIndex
            );

            bestPosition =
                Math.min(
                    bestPosition,
                    bestIndex
                );
        }
    }

    if (matchedWords === 0) {

        return {
            song,
            score: 0,
            bestPosition: Infinity,
            bestMatchText: ""
        };
    }

    const coverage =
        matchedWords /
        queryWords.length;

    const similarity =
        totalSimilarity /
        matchedWords;

    let sequenceScore = 0;

    if (
        matchedPositions.length >= 2
    ) {

        let orderedPairs = 0;
        let closePairs = 0;

        for (
            let i = 1;
            i < matchedPositions.length;
            i++
        ) {

            const previous =
                matchedPositions[i - 1];

            const current =
                matchedPositions[i];

            if (current > previous) {
                orderedPairs++;
            }

            const distance =
                current - previous;

            if (
                distance > 0 &&
                distance <= 4
            ) {
                closePairs++;
            }
        }

        const possible =
            matchedPositions.length - 1;

        sequenceScore =
            (
                orderedPairs /
                possible
            ) * 0.5
            +
            (
                closePairs /
                possible
            ) * 0.5;
    }

    else {

        sequenceScore = 0.5;
    }

    const positionRatio =
        bestPosition /
        Math.max(
            lyricWords.length,
            1
        );

    const beginningScore =
        Math.max(
            0,
            1 - positionRatio
        );

    let finalScore =
        (
            coverage * 0.38 +
            similarity * 0.25 +
            sequenceScore * 0.22 +
            beginningScore * 0.15
        ) * 100;

    const normalizedQuery =
        normalizeText(
            queryWords.join(" ")
        );

    const normalizedLyrics =
        normalizeText(
            song.lyrics
        );

    if (
        normalizedLyrics.includes(
            normalizedQuery
        )
    ) {

        finalScore += 8;
    }

    finalScore =
        Math.min(
            100,
            Math.round(finalScore)
        );

    const bestMatchText =
        getBestMatchText(
            song.lyrics,
            bestPosition
        );

    return {
        song,
        score: finalScore,
        bestPosition,
        bestMatchText
    };
}

/*
========================================
SHOW RESULTS
========================================
*/

function showResults(results) {

    resultsBox.innerHTML = "";

    if (results.length === 0) {

        searchInfo.textContent =
            "No matching songs found.";

        return;
    }

    searchInfo.textContent =
        `${results.length} possible song${
            results.length === 1
                ? ""
                : "s"
        }`;

    for (const result of results) {

        const song =
            result.song;

        const opening =
            getOpeningLines(
                song.lyrics,
                OPENING_LINES
            );

        const openingNormalized =
            normalizeText(opening);

        const matchNormalized =
            normalizeText(
                result.bestMatchText
            );

        const showBestMatch =
            result.bestMatchText &&
            !openingNormalized.includes(
                matchNormalized
            ) &&
            !matchNormalized.includes(
                openingNormalized
            );

        const card =
            document.createElement(
                "div"
            );

        card.className = "result";

        card.innerHTML = `

            <div class="result-heading">

                <span>
                    ${escapeHtml(
                        song.title
                    )}
                </span>

                <span class="score">
                    ${result.score}%
                </span>

            </div>

            <div class="label">
                Beginning
            </div>

            <div class="opening">
                ${escapeHtml(
                    opening
                )}
            </div>

            ${
                showBestMatch
                    ?
                    `
                    <div class="best-match">

                        <div class="label">
                            Best match
                        </div>

                        ${escapeHtml(
                            result.bestMatchText
                        )}

                    </div>
                    `
                    :
                    ""
            }
        `;

        card.addEventListener(
            "click",
            function () {
                openSong(song);
            }
        );

        resultsBox.appendChild(
            card
        );
    }
}


/*
========================================
FULL LYRICS POPUP
========================================
*/

function openSong(song) {

    selectedSong = song;

    popupTitle.textContent =
        song.title;

    popupLyrics.textContent =
        song.lyrics;

    overlay.style.display =
        "flex";
}


function closePopup() {

    overlay.style.display =
        "none";
}


cancelButton.addEventListener(
    "click",
    closePopup
);


overlay.addEventListener(
    "click",
    function (event) {

        if (event.target === overlay) {
            closePopup();
        }
    }
);


/*
========================================
DISPLAY FULL SONG
========================================
*/

displayButton.addEventListener(
    "click",
    function () {

        if (!selectedSong)
            return;

        localStorage.setItem(
            "churchDisplayLyrics",
            selectedSong.lyrics
        );

        localStorage.setItem(
            "churchDisplayTitle",
            selectedSong.title
        );

        closePopup();

        searchBox.value = "";

        resultsBox.innerHTML = "";

        searchInfo.textContent =
            "Start typing to find a song.";

        searchBox.focus();
    }
);

/*
========================================
OPENING PREVIEW
========================================
*/

function getOpeningLines(
    lyrics,
    numberOfLines
) {

    return lyrics
        .split(/\r?\n/)
        .map(
            line => line.trim()
        )
        .filter(
            line => line.length > 0
        )
        .slice(
            0,
            numberOfLines
        )
        .join("\n");
}


/*
========================================
BEST MATCH PREVIEW
========================================
*/

function getBestMatchText(
    lyrics,
    wordPosition
) {

    const words =
        lyrics
            .replace(/\s+/g, " ")
            .trim()
            .split(" ");

    if (
        !Number.isFinite(
            wordPosition
        ) ||
        words.length === 0
    ) {
        return "";
    }

    const start =
        Math.max(
            0,
            wordPosition - 5
        );

    const end =
        Math.min(
            words.length,
            wordPosition + 10
        );

    let preview =
        words
            .slice(start, end)
            .join(" ");

    if (start > 0) {
        preview =
            "..." + preview;
    }

    if (end < words.length) {
        preview += "...";
    }

    return preview;
}


/*
========================================
TEXT CLEANING
========================================
*/

function normalizeText(text) {

    return text
        .toLowerCase()
        .normalize("NFD")
        .replace(
            /[\u0300-\u036f]/g,
            ""
        )
        .replace(
            /[^a-z0-9\s']/g,
            " "
        )
        .replace(
            /\s+/g,
            " "
        )
        .trim();
}


function tokenize(text) {

    return normalizeText(text)
        .split(" ")
        .filter(Boolean);
}


function tokenizeWithPositions(
    text
) {

    return tokenize(text)
        .map(
            (word, index) => ({
                word,
                index
            })
        );
}

/*
========================================
FUZZY WORD MATCHING
========================================
*/

function wordSimilarity(a, b) {

    if (a === b) {
        return 1;
    }

    /*
        Match words while they are
        still being typed.
    */

    if (b.startsWith(a)) {

        if (a.length === 1)
            return 0.62;

        if (a.length === 2)
            return 0.72;

        if (a.length === 3)
            return 0.82;

        if (a.length >= 4)
            return 0.92;
    }

    /*
        Avoid fuzzy matching unrelated
        very short words.
    */

    if (
        a.length <= 2 ||
        b.length <= 2
    ) {
        return 0;
    }

    const distance =
        levenshtein(a, b);

    const longest =
        Math.max(
            a.length,
            b.length
        );

    return Math.max(
        0,
        1 - distance / longest
    );
}


/*
========================================
LEVENSHTEIN DISTANCE
========================================
*/

function levenshtein(a, b) {

    const matrix = [];

    for (
        let i = 0;
        i <= b.length;
        i++
    ) {
        matrix[i] = [i];
    }

    for (
        let j = 0;
        j <= a.length;
        j++
    ) {
        matrix[0][j] = j;
    }

    for (
        let i = 1;
        i <= b.length;
        i++
    ) {

        for (
            let j = 1;
            j <= a.length;
            j++
        ) {

            if (
                b.charAt(i - 1) ===
                a.charAt(j - 1)
            ) {

                matrix[i][j] =
                    matrix[i - 1][j - 1];
            }

            else {

                matrix[i][j] =
                    Math.min(

                        matrix[i - 1][j - 1]
                        + 1,

                        matrix[i][j - 1]
                        + 1,

                        matrix[i - 1][j]
                        + 1
                    );
            }
        }
    }

    return matrix[
        b.length
    ][
        a.length
    ];
}


/*
========================================
SAFE HTML
========================================
*/

function escapeHtml(text) {

    const div =
        document.createElement(
            "div"
        );

    div.textContent = text;

    return div.innerHTML;
}