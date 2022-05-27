# Flashcards
[![.NET](https://github.com/luk-starzec/Flashcards/actions/workflows/dotnet.yml/badge.svg)](https://github.com/luk-starzec/Flashcards/actions/workflows/dotnet.yml)

Flashcards for learning foreign alphabets (japanese hiragana and katakana, korean hangul and cyrillic).

The app includes a learning and a knowledge test mode. It gives many configuration options, e.g. quiz settings or changing the language and colors of the user interface.

Flashcards can function both as a website in the browser and as an offline application installed on a mobile / desktop (as PWA).

The application is made with Blazor Webassembly. Server side is web API providing courses data. When the course is downloaded, all its data are stored client side in IndexedDB (accessed with Entity Framework Core). For other information (e.g. settings) the Local Storage is used.
