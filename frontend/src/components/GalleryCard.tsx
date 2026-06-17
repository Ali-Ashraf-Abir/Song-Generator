import { SongDto } from '@/types';
import LikeBadge from './LikeBadge';
import { Music } from 'lucide-react';

interface GalleryCardProps {
  song: SongDto;
  locale: string;
}

export default function GalleryCard({ song, locale }: GalleryCardProps) {
  return (
    <div className="gallery-card">
      <div className="gallery-cover">
        <img
          src={`/api/covers?seed=${song.index}&index=${song.index}&locale=${locale}`}
          alt={`${song.title} cover`}
          className="gallery-cover-img"
          loading="lazy"
          onError={e => {
            const el = e.target as HTMLImageElement;
            el.style.display = 'none';
            el.parentElement!.classList.add('no-cover');
          }}
        />
        <div className="gallery-cover-placeholder">
          <Music size={32} className="cover-placeholder-icon" />
        </div>
        <div className="gallery-overlay">
          <span className="gallery-index">#{song.index}</span>
          {song.likes > 0 && <LikeBadge count={song.likes} />}
        </div>
      </div>
      <div className="gallery-info">
        <p className="gallery-title">{song.title}</p>
        <p className="gallery-artist">{song.artist}</p>
        <p className={`gallery-album ${song.album === 'Single' ? 'text-muted' : ''}`}>
          {song.album}
        </p>
        <span className="genre-tag small">{song.genre}</span>
      </div>
    </div>
  );
}
