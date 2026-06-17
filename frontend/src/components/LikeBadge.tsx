import { Heart } from 'lucide-react';

export default function LikeBadge({ count }: { count: number }) {
  if (count === 0) return null;
  return (
    <span className="like-badge">
      <Heart size={11} fill="currentColor" />
      {count}
    </span>
  );
}
