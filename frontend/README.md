# MusicVault — Next.js Frontend

A music store showcase SPA that pairs with the .NET backend.

## Setup

```bash
npm install
npm run dev
```

Opens at http://localhost:3000. The `next.config.js` proxies `/api/*` → `http://localhost:5000/api/*`, so the backend must be running.

## Project Structure

```
src/
├── app/
│   ├── layout.tsx        # Root layout + metadata
│   ├── page.tsx          # Main page shell — state lives here
│   └── globals.css       # All styles (CSS variables, components)
├── components/
│   ├── Toolbar.tsx       # Language, seed, likes, view-mode controls
│   ├── TableView.tsx     # Paginated table with expandable rows
│   ├── GalleryView.tsx   # Infinite-scroll card grid
│   ├── ExpandedRow.tsx   # Slide-open detail: cover, player, review
│   ├── GalleryCard.tsx   # Single card for gallery grid
│   └── LikeBadge.tsx     # ❤ count badge
├── hooks/
│   ├── useSongs.ts       # Fetches one page; resets on param change
│   ├── useInfiniteSongs.ts # Accumulates pages; resets on param change
│   └── useDebounce.ts    # Debounces seed input & slider
├── lib/
│   └── api.ts            # fetch wrappers for /api/song & /api/covers
└── types/
    └── index.ts          # SongDto, ViewMode, AppParams
```

## Behaviour

| Action | Result |
|--------|--------|
| Change locale | Table resets to p.1; gallery resets scroll |
| Change seed | Same as locale |
| Drag likes slider | Only like counts update; titles/artists unchanged |
| Click table row | Expands in-place (click again to collapse) |
| Scroll to bottom in gallery | Loads next batch automatically |
| Click shuffle icon | Randomises seed |

## Backend API expected

`GET /api/song?locale=en-US&seed=12345&page=1&pageSize=10&avgLikes=3.7`  
→ `SongDto[]`

`GET /api/covers?seed=&index=&locale=`  
→ PNG image

## Adding languages

Add an entry to the `LOCALES` array in `Toolbar.tsx`. No other changes needed — the backend handles locale data.
