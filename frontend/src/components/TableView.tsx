'use client';

import { useState } from 'react';
import { ChevronDown, ChevronUp, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-react';
import { SongDto } from '@/types';
import { useSongs } from '@/hooks/useSongs';
import ExpandedRow from './ExpandedRow';

interface TableViewProps {
  locale: string;
  seed: string;
  avgLikes: number;
}

export default function TableView({ locale, seed, avgLikes }: TableViewProps) {
  const { songs, page, setPage, loading, error, pageSize } = useSongs(locale, seed, avgLikes);
  const [expanded, setExpanded] = useState<number | null>(null);

  const toggleExpand = (index: number) => {
    setExpanded(prev => prev === index ? null : index);
  };

  if (error) {
    return (
      <div className="error-state">
        <p>Failed to load songs: {error}</p>
        <p className="error-hint">Make sure the backend is running on port 5000.</p>
      </div>
    );
  }

  return (
    <div className="table-container">
      <table className="song-table">
        <thead>
          <tr>
            <th className="col-expand" />
            <th className="col-num">#</th>
            <th className="col-song">Song</th>
            <th className="col-artist">Artist</th>
            <th className="col-album">Album</th>
            <th className="col-genre">Genre</th>
          </tr>
        </thead>
        <tbody>
          {loading && songs.length === 0 ? (
            Array.from({ length: pageSize }).map((_, i) => (
              <tr key={i} className="skeleton-row">
                <td /><td /><td><div className="skeleton" /></td>
                <td><div className="skeleton" /></td>
                <td><div className="skeleton" /></td>
                <td><div className="skeleton short" /></td>
              </tr>
            ))
          ) : (
            songs.map(song => {
              const isExpanded = expanded === song.index;
              return (
                <>
                  <tr
                    key={song.index}
                    className={`song-row ${isExpanded ? 'song-row--expanded' : ''}`}
                    onClick={() => toggleExpand(song.index)}
                  >
                    <td className="col-expand">
                      {isExpanded
                        ? <ChevronUp size={14} className="expand-icon" />
                        : <ChevronDown size={14} className="expand-icon" />
                      }
                    </td>
                    <td className="col-num font-mono">{song.index}</td>
                    <td className="col-song">{song.title}</td>
                    <td className="col-artist">{song.artist}</td>
                    <td className={`col-album ${song.album === 'Single' ? 'text-muted' : ''}`}>
                      {song.album}
                    </td>
                    <td className="col-genre">
                      <span className="genre-tag">{song.genre}</span>
                    </td>
                  </tr>
                  {isExpanded && (
                    <tr key={`expanded-${song.index}`} className="expanded-tr">
                      <td colSpan={6} className="expanded-td">
                        <ExpandedRow song={song} locale={locale} />
                      </td>
                    </tr>
                  )}
                </>
              );
            })
          )}
        </tbody>
      </table>

      {/* Pagination */}
      <div className="pagination">
        <button
          className="page-btn"
          onClick={() => setPage(1)}
          disabled={page === 1 || loading}
          title="First page"
        >
          <ChevronsLeft size={14} />
        </button>
        <button
          className="page-btn"
          onClick={() => setPage(p => Math.max(1, p - 1))}
          disabled={page === 1 || loading}
          title="Previous page"
        >
          <ChevronLeft size={14} />
        </button>

        {/* Page numbers around current */}
        {[page - 1, page, page + 1].filter(p => p >= 1).map(p => (
          <button
            key={p}
            className={`page-btn page-num ${p === page ? 'active' : ''}`}
            onClick={() => setPage(p)}
            disabled={loading}
          >
            {p}
          </button>
        ))}

        <button
          className="page-btn"
          onClick={() => setPage(p => p + 1)}
          disabled={loading}
          title="Next page"
        >
          <ChevronRight size={14} />
        </button>
      </div>
    </div>
  );
}
