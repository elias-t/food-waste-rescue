import { Link } from 'react-router-dom';
import { Plus, Package } from 'lucide-react';
import { useMyListings } from '../hooks/useMyListings';
import { Button } from '../components/ui/Button';
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

const statusStyles: Record<ListingStatus, string> = {
  Active: 'bg-green-100 text-green-800',
  Claimed: 'bg-blue-100 text-blue-800',
  Expired: 'bg-gray-100 text-gray-600',
  Cancelled: 'bg-red-100 text-red-800',
};

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString(undefined, {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  });
}

function LoadingSkeleton() {
  return (
    <>
      {Array.from({ length: 4 }).map((_, i) => (
        <tr key={i} className="animate-pulse">
          <td className="px-4 py-3"><div className="h-4 bg-gray-200 rounded w-40" /></td>
          <td className="px-4 py-3"><div className="h-4 bg-gray-200 rounded w-20" /></td>
          <td className="px-4 py-3"><div className="h-4 bg-gray-200 rounded w-16" /></td>
          <td className="px-4 py-3"><div className="h-4 bg-gray-200 rounded w-24" /></td>
          <td className="px-4 py-3"><div className="h-4 bg-gray-200 rounded w-8" /></td>
          <td className="px-4 py-3"><div className="h-4 bg-gray-200 rounded w-24" /></td>
        </tr>
      ))}
    </>
  );
}

export default function MyListingsPage() {
  const { data: listings = [], isLoading, isError } = useMyListings();

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">My Listings</h1>
          {!isLoading && (
            <p className="text-sm text-gray-500 mt-0.5">{listings.length} total</p>
          )}
        </div>
        <Link to="/create-listing">
          <Button size="sm">
            <Plus className="h-4 w-4 mr-1.5" />
            New listing
          </Button>
        </Link>
      </div>

      {isError && (
        <div className="py-8 text-center text-sm text-red-500">
          Failed to load your listings. Please try again.
        </div>
      )}

      {!isLoading && !isError && listings.length === 0 && (
        <div className="py-16 text-center">
          <Package className="mx-auto h-10 w-10 text-gray-300 mb-3" />
          <p className="text-sm text-gray-500 mb-4">You haven't created any listings yet.</p>
          <Link to="/create-listing">
            <Button variant="secondary" size="sm">Create your first listing</Button>
          </Link>
        </div>
      )}

      {(isLoading || listings.length > 0) && (
        <div className="bg-white rounded-xl shadow-sm ring-1 ring-gray-100 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Title</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Category</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Status</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Expires</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Claims</th>
                <th className="px-4 py-3 text-left font-medium text-gray-600">Created</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {isLoading ? (
                <LoadingSkeleton />
              ) : (
                listings.map((listing) => (
                  <tr key={listing.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3">
                      <Link
                        to={`/listings/${listing.id}`}
                        className="font-medium text-gray-900 hover:text-primary transition-colors"
                      >
                        {listing.title}
                      </Link>
                    </td>
                    <td className="px-4 py-3 text-gray-600">
                      {categoryLabels[listing.category]}
                    </td>
                    <td className="px-4 py-3">
                      <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${statusStyles[listing.status]}`}>
                        {listing.status}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-600">{formatDate(listing.expiresAt)}</td>
                    <td className="px-4 py-3 text-gray-600">{listing.claimCount}</td>
                    <td className="px-4 py-3 text-gray-600">{formatDate(listing.createdAt)}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
