'use client';

import { useCallback } from 'react';
import { Shuffle, LayoutGrid, TableIcon } from 'lucide-react';
import { ViewMode } from '@/types';

const LOCALES = [
  { code: 'en-US', label: 'English (US)' },
  { code: 'de-DE', label: 'German (DE)' },
];

interface ToolbarProps {
  locale: string;
  seed: string;
  avgLikes: number;
  viewMode: ViewMode;
  onLocaleChange: (v: string) => void;
  onSeedChange: (v: string) => void;
  onAvgLikesChange: (v: number) => void;
  onViewModeChange: (v: ViewMode) => void;
}

export default function Toolbar({
  locale,
  seed,
  avgLikes,
  viewMode,
  onLocaleChange,
  onSeedChange,
  onAvgLikesChange,
  onViewModeChange,
}: ToolbarProps) {
  const randomizeSeed = useCallback(() => {
    const rand = Math.floor(Math.random() * 99999999) + 1000000;
    onSeedChange(String(rand));
  }, [onSeedChange]);

  return (
    <div className="toolbar">
      {/* Language */}
      <div className="toolbar-group">
        <label className="toolbar-label">Language</label>
        <select
          value={locale}
          onChange={e => onLocaleChange(e.target.value)}
          className="toolbar-select"
        >
          {LOCALES.map(l => (
            <option key={l.code} value={l.code}>{l.label}</option>
          ))}
        </select>
      </div>

      {/* Seed */}
      <div className="toolbar-group">
        <label className="toolbar-label">Seed</label>
        <div className="seed-input-wrap">
          <input
            type="text"
            value={seed}
            onChange={e => onSeedChange(e.target.value.replace(/\D/g, '').slice(0, 18))}
            className="toolbar-input"
            placeholder="Enter seed…"
          />
          <button
            onClick={randomizeSeed}
            className="seed-btn"
            title="Randomize seed"
          >
            <Shuffle size={14} />
          </button>
        </div>
      </div>

      {/* Likes */}
      <div className="toolbar-group">
        <label className="toolbar-label">
          Likes&nbsp;<span className="likes-value">{avgLikes.toFixed(1)}</span>
        </label>
        <input
          type="range"
          min={0}
          max={10}
          step={0.1}
          value={avgLikes}
          onChange={e => onAvgLikesChange(Number(e.target.value))}
          className="likes-slider"
        />
      </div>

      {/* Spacer */}
      <div style={{ flex: 1 }} />

      {/* View mode */}
      <div className="view-toggle">
        <button
          onClick={() => onViewModeChange('table')}
          className={`view-btn ${viewMode === 'table' ? 'active' : ''}`}
          title="Table view"
        >
          <TableIcon size={16} />
        </button>
        <button
          onClick={() => onViewModeChange('gallery')}
          className={`view-btn ${viewMode === 'gallery' ? 'active' : ''}`}
          title="Gallery view"
        >
          <LayoutGrid size={16} />
        </button>
      </div>
    </div>
  );
}
