import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import L from 'leaflet';
import '../../utils/leafletIconFix';

const defaultIcon = new L.Icon.Default();

interface Location {
  latitude: number;
  longitude: number;
}

interface LocationMarkerProps {
  value: Location | null;
  onChange: (loc: Location) => void;
}

function LocationMarker({ value, onChange }: LocationMarkerProps) {
  useMapEvents({
    click(e) {
      onChange({ latitude: e.latlng.lat, longitude: e.latlng.lng });
    },
  });
  return value
    ? <Marker position={[value.latitude, value.longitude]} icon={defaultIcon} />
    : null;
}

interface LocationPickerProps {
  value: Location | null;
  onChange: (loc: Location) => void;
}

export default function LocationPicker({ value, onChange }: LocationPickerProps) {
  const center: [number, number] = value
    ? [value.latitude, value.longitude]
    : [37.9838, 23.7275];

  return (
    <MapContainer
      center={center}
      zoom={13}
      style={{ height: '300px', width: '100%' }}
      scrollWheelZoom={false}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <LocationMarker value={value} onChange={onChange} />
    </MapContainer>
  );
}
