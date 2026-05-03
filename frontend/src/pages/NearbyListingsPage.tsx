import { useState, useRef } from 'react';
import { MapPin } from 'lucide-react';
import { useGeolocation } from '../hooks/useGeolocation';
import { useNearbyListings } from '../hooks/useNearbyListings';
import ListingCard from '../components/listings/ListingCard';
import ListingMap from '../components/listings/ListingMap';

function LoadingSkeleton() {
  return (
    <>
      {Array.from({ length: 5 }).map((_, i) => (
        <div key={i} className="animate-pulse bg-white rounded-xl p-4 shadow-sm ring-1 ring-gray-100">
          <div className="flex gap-2 mb-2">
            <div className="h-4 bg-gray-200 rounded flex-1" />
            <div className="h-4 bg-gray-200 rounded w-16" />
          </div>
          <div className="h-3 bg-gray-200 rounded w-2/3 mb-3" />
          <div className="flex gap-4">
            <div className="h-3 bg-gray-200 rounded w-10" />
            <div className="h-3 bg-gray-200 rounded w-20" />
            <div className="h-3 bg-gray-200 rounded w-16" />
          </div>
        </div>
      ))}
    </>
  );
}

export default function NearbyListingsPage() {
  const { latitude, longitude, loading: geoLoading } = useGeolocation();
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const cardRefs = useRef<Record<string, HTMLDivElement | null>>({});

  const { data: listings = [], isLoading, isError } = useNearbyListings({ latitude, longitude });

  const center = { lat: latitude ?? 37.9838, lng: longitude ?? 23.7275 };
  const showSkeleton = geoLoading || isLoading;

  const handleMarkerClick = (id: string) => {
    setSelectedId(id);
    cardRefs.current[id]?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  };

  return (
    <div className="flex flex-col lg:grid lg:grid-cols-3 gap-4">
      {/* Sidebar */}
      <div className="flex flex-col gap-3">
        <div className="flex items-baseline gap-2">
          <h1 className="text-xl font-bold text-gray-900">Nearby Listings</h1>
          {!showSkeleton && (
            <span className="text-sm text-gray-500">{listings.length} found</span>
          )}
        </div>

        <div className="max-h-[80vh] lg:max-h-[560px] overflow-y-auto flex flex-col gap-3 pr-1">
          {showSkeleton && <LoadingSkeleton />}

          {!showSkeleton && isError && (
            <div className="py-8 text-center text-sm text-red-500">
              Failed to load listings. Please try again.
            </div>
          )}

          {!showSkeleton && !isError && listings.length === 0 && (
            <div className="py-12 text-center">
              <MapPin className="mx-auto h-10 w-10 text-gray-300 mb-3" />
              <p className="text-sm text-gray-500">No listings found within 10 km</p>
            </div>
          )}

          {listings.map((listing) => (
            <div
              key={listing.id}
              ref={(el) => { cardRefs.current[listing.id] = el; }}
            >
              <ListingCard
                listing={listing}
                isSelected={listing.id === selectedId}
                onClick={() => setSelectedId(listing.id)}
              />
            </div>
          ))}
        </div>
      </div>

      {/* Map — only mount after geolocation resolves so the initial center is correct */}
      <div className="lg:col-span-2 h-[400px] lg:h-[600px] rounded-xl overflow-hidden shadow-sm bg-gray-200">
        {!geoLoading && (
          <ListingMap
            listings={listings}
            center={center}
            selectedId={selectedId}
            onMarkerClick={handleMarkerClick}
          />
        )}
      </div>
    </div>
  );
}
