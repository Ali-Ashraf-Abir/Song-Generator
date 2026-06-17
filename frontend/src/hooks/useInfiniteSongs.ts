import { useState, useEffect, useCallback, useRef } from 'react';
import { SongDto } from '@/types';
import { fetchSongs } from '@/lib/api';

const PAGE_SIZE = 20;

export function useInfiniteSongs(locale: string, seed: string, avgLikes: number) {
  const [songs, setSongs] = useState<SongDto[]>([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const loadingRef = useRef(false);

  // Reset when params change
  useEffect(() => {
    setSongs([]);
    setPage(1);
    loadingRef.current = false;
  }, [locale, seed, avgLikes]);

  const loadPage = useCallback(async (p: number, reset: boolean) => {
    if (loadingRef.current) return;
    loadingRef.current = true;
    setLoading(true);
    setError(null);
    try {
      const data = await fetchSongs(locale, seed, p, PAGE_SIZE, avgLikes);
      setSongs(prev => reset ? data : [...prev, ...data]);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Unknown error');
    } finally {
      setLoading(false);
      loadingRef.current = false;
    }
  }, [locale, seed, avgLikes]);

  // Load initial page
  useEffect(() => {
    loadPage(1, true);
  }, [loadPage]);

  const loadMore = useCallback(() => {
    const next = page + 1;
    setPage(next);
    loadPage(next, false);
  }, [page, loadPage]);

  return { songs, loading, error, loadMore };
}
