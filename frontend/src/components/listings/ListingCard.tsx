import { MapPin, Clock, Store } from 'lucide-react';
import { Link } from 'react-router-dom';
import type { NearbyListing, FoodCategory } from '../../types/api';

function formatTimeUntil(isoDate: string): string {
  const diff = new Date(isoDate).getTime() - Date.now();
  if (diff <= 0) return 'Expired';
  const totalMinutes = Math.floor(diff / 60_000);
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;
  if (hours === 0) return `${minutes}m`;
  if (hours < 24) return `${hours}h ${minutes}m`;
  return `${Math.floor(hours / 24)}d`;
}

function formatDistance(km: number): string {
  return km < 1 ? `${Math.round(km * 1000)} m` : `${km.toFixed(1)} km`;
}

const categoryStyles: Record<FoodCategory, string> = {
  Bakery: 'bg-amber-100 text-amber-800',
  Dairy: 'bg-blue-100 text-blue-800',
  FruitsAndVegetables: 'bg-green-100 text-green-800',
  MeatAndFish: 'bg-red-100 text-red-800',
  PreparedMeals: 'bg-orange-100 text-orange-800',
  Pantry: 'bg-yellow-100 text-yellow-800',
  Beverages: 'bg-cyan-100 text-cyan-800',
  Other: 'bg-gray-100 text-gray-800',
};

const categoryLabels: Record<FoodCategory, string> = {
  Bakery: 'Bakery',
  Dairy: 'Dairy',
  FruitsAndVegetables: 'Fruits & Veg',
  MeatAndFish: 'Meat & Fish',
  PreparedMeals: 'Prepared Meals',
  Pantry: 'Pantry',
  Beverages: 'Beverages',
  Other: 'Other',
};

interface ListingCardProps {
  listing: NearbyListing;
  isSelected?: boolean;
  onClick?: () => void;
}

export default function ListingCard({ listing, isSelected = false, onClick }: ListingCardProps) {
  const timeUntil = formatTimeUntil(listing.expiresAt);
  const isExpired = timeUntil === 'Expired';

  return (
    <div
      onClick={onClick}
      className={[
        'bg-white rounded-xl p-4 shadow-sm cursor-pointer transition-all duration-150',
        'hover:shadow-md',
        isSelected ? 'ring-2 ring-primary' : 'ring-1 ring-gray-100',
      ].join(' ')}
    >
      <div className="flex items-start justify-between gap-2 mb-2">
        <Link
          to={`/listings/${listing.id}`}
          onClick={(e) => e.stopPropagation()}
          className="font-semibold text-gray-900 hover:text-primary transition-colors leading-tight"
        >
          {listing.title}
        </Link>
        <span className={`shrink-0 text-xs font-medium px-2 py-0.5 rounded-full ${categoryStyles[listing.category]}`}>
          {categoryLabels[listing.category]}
        </span>
      </div>

      <p className="text-sm text-gray-600 mb-3">{listing.quantityDescription}</p>

      <div className="flex flex-wrap gap-x-4 gap-y-1 text-xs text-gray-500">
        <span className="flex items-center gap-1">
          <MapPin className="h-3.5 w-3.5" />
          {formatDistance(listing.distanceKm)}
        </span>
        <span className={`flex items-center gap-1 ${isExpired ? 'text-red-500' : ''}`}>
          <Clock className="h-3.5 w-3.5" />
          {isExpired ? 'Expired' : `Expires in ${timeUntil}`}
        </span>
        <span className="flex items-center gap-1">
          <Store className="h-3.5 w-3.5" />
          {listing.donorName}
        </span>
      </div>
    </div>
  );
}
