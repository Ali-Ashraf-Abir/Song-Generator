import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'MusicVault — Song Showcase',
  description: 'Discover procedurally generated music across every genre and language.',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
