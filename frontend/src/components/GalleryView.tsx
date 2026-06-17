'use client';

import { useEffect } from 'react';
import { useInView } from 'react-intersection-observer';
import { useInfiniteSongs } from '@/hooks/useInfiniteSongs';
import GalleryCard from './GalleryCard';
import { Loader2 } from 'lucide-react';

interface GalleryViewProps {
  locale: string;
  seed: string;
  avgLikes: number;
}

export default function GalleryView({ locale, seed, avgLikes }: GalleryViewProps) {
  const { songs, loading, error, loadMore } = useInfiniteSongs(locale, seed, avgLikes);
  const { ref, inView } = useInView({ threshold: 0.1, rootMargin: '200px' });

  useEffect(() => {
    if (inView && !loading) {
      loadMore();
    }
  }, [inView, loading, loadMore]);

  if (error) {
    return (
      <div className="error-state">
        <p>Failed to load songs: {error}</p>
        <p className="error-hint">Make sure the backend is running on port 5000.</p>
      </div>
    );
  }

  return (
    <div className="gallery-container">
      <div className="gallery-grid">
        {songs.map(song => (
          <GalleryCard key={song.index} song={song} locale={locale} />
        ))}
        {loading && songs.length === 0 &&
          Array.from({ length: 20 }).map((_, i) => (
            <div key={i} className="gallery-card skeleton-card">
              <div className="gallery-cover skeleton-cover" />
              <div className="gallery-info">
                <div className="skeleton" style={{ height: 14, marginBottom: 6 }} />
                <div className="skeleton" style={{ height: 12, width: '70%' }} />
              </div>
            </div>
          ))
        }
      </div>

      {/* Sentinel for infinite scroll */}
      <div ref={ref} className="scroll-sentinel">
        {loading && songs.length > 0 && (
          <div className="loading-more">
            <Loader2 size={18} className="spin" />
            <span>Loading more…</span>
          </div>
        )}
      </div>
    </div>
  );
}
