export interface SongDto {
  index: number;
  title: string;
  artist: string;
  album: string;
  genre: string;
  likes: number;
  coverUrl: string;
}

export type ViewMode = 'table' | 'gallery';

export interface AppParams {
  locale: string;
  seed: string;
  avgLikes: number;
}

export interface PagedSongs {
  songs: SongDto[];
  page: number;
}
