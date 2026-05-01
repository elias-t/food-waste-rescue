import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import { Link } from 'react-router-dom';
import L from 'leaflet';
import iconRetinaUrl from 'leaflet/dist/images/marker-icon-2x.png';
import iconUrl from 'leaflet/dist/images/marker-icon.png';
import shadowUrl from 'leaflet/dist/images/marker-shadow.png';
import type { NearbyListing } from '../../types/api';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({ iconRetinaUrl, iconUrl, shadowUrl });

const defaultIcon = new L.Icon.Default();

const selectedIcon = L.divIcon({
  className: '',
  html: '<div style="width:22px;height:22px;background:#16a34a;border:3px solid white;border-radius:50%;box-shadow:0 2px 6px rgba(0,0,0,0.4)"></div>',
  iconSize: [22, 22],
  iconAnchor: [11, 11],
  popupAnchor: [0, -13],
});

interface Center {
  lat: number;
  lng: number;
}

interface ListingMapProps {
  listings: NearbyListing[];
  center: Center;
  selectedId: string | null;
  onMarkerClick: (id: string) => void;
}

export default function ListingMap({ listings, center, selectedId, onMarkerClick }: ListingMapProps) {
  return (
    <MapContainer
      center={[center.lat, center.lng]}
      zoom={13}
      style={{ height: '100%', width: '100%' }}
      scrollWheelZoom={true}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {listings.map((listing) => {
        if (listing.latitude === null || listing.longitude === null) return null;
        const isSelected = listing.id === selectedId;
        return (
          <Marker
            key={listing.id}
            position={[listing.latitude, listing.longitude]}
            icon={isSelected ? selectedIcon : defaultIcon}
            zIndexOffset={isSelected ? 1000 : 0}
            eventHandlers={{ click: () => onMarkerClick(listing.id) }}
          >
            <Popup>
              <div className="text-sm min-w-[140px]">
                <p className="font-semibold mb-1">{listing.title}</p>
                <Link to={`/listings/${listing.id}`} className="text-primary hover:underline">
                  View details →
                </Link>
              </div>
            </Popup>
          </Marker>
        );
      })}
    </MapContainer>
  );
}
