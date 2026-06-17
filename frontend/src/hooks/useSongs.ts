import { useState, useEffect, useCallback } from 'react';
import { SongDto } from '@/types';
import { fetchSongs } from '@/lib/api';

const PAGE_SIZE = 10;

export function useSongs(locale: string, seed: string, avgLikes: number) {
  const [songs, setSongs] = useState<SongDto[]>([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (p: number) => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchSongs(locale, seed, p, PAGE_SIZE, avgLikes);
      setSongs(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  }, [locale, seed, avgLikes]);

  // Reset to page 1 when params change
  useEffect(() => {
    setPage(1);
  }, [locale, seed, avgLikes]);

  useEffect(() => {
    load(page);
  }, [load, page]);

  return { songs, page, setPage, loading, error, pageSize: PAGE_SIZE };
}
