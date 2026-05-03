import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft, MapPin, Clock, Store, Building2, Package, Users } from 'lucide-react';
import { useListingDetail } from '../hooks/useListingDetail';
import { ClaimModal } from '../components/listings/ClaimModal';
import { Button } from '../components/ui/Button';
import { useAuth } from '../store/useAuth';
import type { FoodCategory, ListingStatus } from '../types/api';

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

const statusStyles: Record<ListingStatus, string> = {
  Active: 'bg-green-100 text-green-800',
  Claimed: 'bg-blue-100 text-blue-800',
  Expired: 'bg-gray-100 text-gray-600',
  Cancelled: 'bg-red-100 text-red-800',
};

function formatExpiry(isoDate: string): { label: string; expired: boolean } {
  const diff = new Date(isoDate).getTime() - Date.now();
  if (diff <= 0) return { label: 'Expired', expired: true };
  const hours = Math.floor(diff / 3_600_000);
  if (hours < 24) return { label: `Expires in ${hours}h`, expired: false };
  return { label: `Expires in ${Math.floor(hours / 24)}d`, expired: false };
}

export default function ListingDetailPage() {
  const { id = '' } = useParams<{ id: string }>();
  const { data: listing, isLoading, isError } = useListingDetail(id);
  const { isAuthenticated, user } = useAuth();
  const [claimOpen, setClaimOpen] = useState(false);

  if (isLoading) {
    return (
      <div className="max-w-2xl mx-auto animate-pulse space-y-4 pt-4">
        <div className="h-7 bg-gray-200 rounded w-2/3" />
        <div className="h-4 bg-gray-200 rounded w-1/3" />
        <div className="h-24 bg-gray-200 rounded" />
      </div>
    );
  }

  if (isError || !listing) {
    return (
      <div className="max-w-2xl mx-auto pt-8 text-center">
        <p className="text-red-500 mb-4">Failed to load listing.</p>
        <Link to="/listings" className="text-sm text-primary hover:underline">
          ← Back to listings
        </Link>
      </div>
    );
  }

  const { label: expiryLabel, expired } = formatExpiry(listing.expiresAt);
  const canClaim = isAuthenticated && user?.role === 'Claimer' && listing.status === 'Active';

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <Link
        to="/listings"
        className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-900 transition-colors"
      >
        <ArrowLeft className="h-4 w-4" />
        Back to listings
      </Link>

      <div className="bg-white rounded-xl shadow-sm ring-1 ring-gray-100 p-6 space-y-5">
        {/* Header */}
        <div className="flex items-start justify-between gap-3">
          <h1 className="text-2xl font-bold text-gray-900 leading-tight">{listing.title}</h1>
          <div className="flex flex-col items-end gap-2 shrink-0">
            <span className={`text-xs font-medium px-2.5 py-1 rounded-full ${categoryStyles[listing.category]}`}>
              {categoryLabels[listing.category]}
            </span>
            <span className={`text-xs font-medium px-2.5 py-1 rounded-full ${statusStyles[listing.status]}`}>
              {listing.status}
            </span>
          </div>
        </div>

        {/* Description */}
        <p className="text-gray-700 leading-relaxed">{listing.description}</p>

        {/* Meta */}
        <dl className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-sm">
          <div className="flex items-center gap-2 text-gray-600">
            <Package className="h-4 w-4 shrink-0 text-gray-400" />
            <span>{listing.quantityDescription}</span>
          </div>
          <div className={`flex items-center gap-2 ${expired ? 'text-red-500' : 'text-gray-600'}`}>
            <Clock className="h-4 w-4 shrink-0 text-gray-400" />
            <span>{expiryLabel}</span>
          </div>
          <div className="flex items-center gap-2 text-gray-600">
            <Store className="h-4 w-4 shrink-0 text-gray-400" />
            <span>{listing.donorName}</span>
          </div>
          {listing.organisationName && (
            <div className="flex items-center gap-2 text-gray-600">
              <Building2 className="h-4 w-4 shrink-0 text-gray-400" />
              <span>{listing.organisationName}</span>
            </div>
          )}
          {listing.address && (
            <div className="flex items-center gap-2 text-gray-600 sm:col-span-2">
              <MapPin className="h-4 w-4 shrink-0 text-gray-400" />
              <span>{listing.address}</span>
            </div>
          )}
          <div className="flex items-center gap-2 text-gray-600">
            <Users className="h-4 w-4 shrink-0 text-gray-400" />
            <span>{listing.claimCount} {listing.claimCount === 1 ? 'claim' : 'claims'}</span>
          </div>
        </dl>

        {/* Actions */}
        {canClaim && (
          <div className="pt-2 border-t border-gray-100">
            <Button size="lg" onClick={() => setClaimOpen(true)}>
              Claim this listing
            </Button>
          </div>
        )}

        {!isAuthenticated && listing.status === 'Active' && (
          <p className="pt-2 border-t border-gray-100 text-sm text-gray-500">
            <Link to="/login" className="text-primary hover:underline">Sign in</Link> to claim this listing.
          </p>
        )}
      </div>

      <ClaimModal isOpen={claimOpen} onClose={() => setClaimOpen(false)} />
    </div>
  );
}
