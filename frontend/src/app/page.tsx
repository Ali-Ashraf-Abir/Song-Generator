'use client';

import { useState, useCallback } from 'react';
import { Disc3 } from 'lucide-react';
import Toolbar from '@/components/Toolbar';
import TableView from '@/components/TableView';
import GalleryView from '@/components/GalleryView';
import { useDebounce } from '@/hooks/useDebounce';
import { ViewMode } from '@/types';

const DEFAULT_SEED = '58933423';

export default function Home() {
  const [locale, setLocale] = useState('en-US');
  const [seed, setSeed] = useState(DEFAULT_SEED);
  const [avgLikes, setAvgLikes] = useState(3.7);
  const [viewMode, setViewMode] = useState<ViewMode>('table');

  // Debounce seed so live-typing doesn't hammer the API
  const debouncedSeed = useDebounce(seed, 350);
  const debouncedLikes = useDebounce(avgLikes, 150);

  const handleViewModeChange = useCallback((m: ViewMode) => setViewMode(m), []);

  return (
    <div className="app-shell">
      <header className="app-header">
        <a href="/" className="app-logo">
          <div className="app-logo-dot" />
          MusicVault
        </a>

        <Toolbar
          locale={locale}
          seed={seed}
          avgLikes={avgLikes}
          viewMode={viewMode}
          onLocaleChange={setLocale}
          onSeedChange={setSeed}
          onAvgLikesChange={setAvgLikes}
          onViewModeChange={handleViewModeChange}
        />
      </header>

      <main className="app-main">
        {viewMode === 'table' ? (
          <TableView
            locale={locale}
            seed={debouncedSeed}
            avgLikes={debouncedLikes}
          />
        ) : (
          <GalleryView
            locale={locale}
            seed={debouncedSeed}
            avgLikes={debouncedLikes}
          />
        )}
      </main>
    </div>
  );
}
