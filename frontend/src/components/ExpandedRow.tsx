'use client';

import { useState } from 'react';
import { Play, Music } from 'lucide-react';
import { SongDto } from '@/types';
import LikeBadge from './LikeBadge';

// Deterministic "review" based on song index
const REVIEW_TEMPLATES = [
  "A bold statement from {artist} — this track doesn't waste a second. The production feels simultaneously vintage and forward-thinking.",
  "{artist} continues to defy genre boundaries. '{title}' is the kind of track you discover at 2am and can't stop replaying.",
  "Understated yet magnetic. '{title}' lets silence do the heavy lifting, trusting listeners to fill the gaps themselves.",
  "A throwback production style elevated by {artist}'s unmistakable sensibility. The chorus alone is worth the price of admission.",
  "Rarely does an artist sound this comfortable taking risks. '{title}' is proof that {artist} still has something to say.",
  "There's a restlessness to this record that feels earned. '{title}' closes the album on an unresolved, haunting note.",
  "Melodically rich and lyrically spare — {artist} knows exactly which details to leave out. One of the year's best.",
];

const LABELS = ['Atlantic', 'Interscope', 'Def Jam', 'XL Recordings', 'Sub Pop', 'Domino', 'Rough Trade', 'Matador', 'Warp', 'Ninja Tune'];
const YEARS = [2019, 2020, 2021, 2022, 2023, 2024];

function pseudoRng(seed: number): () => number {
  let s = seed;
  return () => {
    s = (s * 1664525 + 1013904223) & 0xffffffff;
    return (s >>> 0) / 0xffffffff;
  };
}

interface ExpandedRowProps {
  song: SongDto;
  locale: string;
}

export default function ExpandedRow({ song, locale }: ExpandedRowProps) {
  const [playing, setPlaying] = useState(false);

  const rng = pseudoRng(song.index * 31 + 7);
  const reviewIdx = Math.floor(rng() * REVIEW_TEMPLATES.length);
  const labelIdx = Math.floor(rng() * LABELS.length);
  const yearIdx = Math.floor(rng() * YEARS.length);

  const review = REVIEW_TEMPLATES[reviewIdx]
    .replace('{artist}', song.artist)
    .replace('{title}', song.title);

  const label = LABELS[labelIdx];
  const year = YEARS[yearIdx];
  const isSingle = song.album === 'Single';
  console.log(song.previewUrl);
  return (
    <div className="expanded-row">
      {/* Cover */}
      <div className="cover-wrap">
        <img
          src={`https://song-generator-ni0j.onrender.com${song.coverUrl}`}
          alt={`${song.title} cover`}
          className="cover-img"
          onError={e => {
            (e.target as HTMLImageElement).style.display = 'none';
          }}
        />
        <LikeBadge count={song.likes} />
      </div>

      {/* Details */}
      <div className="expanded-details">
        <div className="expanded-meta">
          <h3 className="expanded-title">{song.title}</h3>
          <p className="expanded-from">
            {isSingle ? (
              <>Single by <strong>{song.artist}</strong></>
            ) : (
              <>from <strong>{song.album}</strong> by <strong>{song.artist}</strong></>
            )}
          </p>
          <p className="expanded-label">{label}, {year}</p>
        </div>

        {/* Fake player */}
        <div className="player-bar">
          <button
            className={`play-btn ${playing ? 'playing' : ''}`}
            onClick={() => setPlaying(p => !p)}
            title={playing ? 'Pause' : 'Play preview'}
          >
            {playing ? (
              <span className="pause-icon">⏸</span>
            ) : (
              <Play size={14} fill="currentColor" />
            )}
          </button>
          <Music size={13} className="player-icon" />
          <div className="player-track">
            <div className="player-progress" style={{ width: playing ? '35%' : '0%' }} />
          </div>
          <span className="player-time">2:{String(Math.floor(30 + (song.index % 30))).padStart(2, '0')}</span>
        </div>
        <audio
          controls
          preload="none"
          src={`http://localhost:5216${song.previewUrl}`}
        />
        {/* Review */}
        <p className="expanded-review">{review}</p>
      </div>
    </div>
  );
}
