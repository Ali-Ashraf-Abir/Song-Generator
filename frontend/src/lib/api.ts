import { SongDto } from '@/types';

const BASE_URL = 'https://song-generator-ni0j.onrender.com/api';

export async function fetchSongs(
  locale: string,
  seed: string,
  page: number,
  pageSize: number,
  avgLikes: number
): Promise<SongDto[]> {
  const params = new URLSearchParams({
    locale,
    seed,
    page: String(page),
    pageSize: String(pageSize),
    avgLikes: String(avgLikes),
  });

  const res = await fetch(`${BASE_URL}/song?${params}`);
  if (!res.ok) throw new Error(`Failed to fetch songs: ${res.statusText}`);
  return res.json();
}

export function getCoverUrl(seed: number, index: number, locale: string) {
  return `${BASE_URL}/covers?seed=${seed}&index=${index}&locale=${locale}`;
}
