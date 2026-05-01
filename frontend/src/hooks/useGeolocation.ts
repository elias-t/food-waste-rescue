import { useState, useEffect } from 'react';

const FALLBACK = { latitude: 37.9838, longitude: 23.7275 };

interface GeolocationState {
  latitude: number | null;
  longitude: number | null;
  error: string | null;
  loading: boolean;
}

export function useGeolocation(): GeolocationState {
  const [state, setState] = useState<GeolocationState>({
    latitude: null,
    longitude: null,
    error: null,
    loading: true,
  });

  useEffect(() => {
    if (!navigator.geolocation) {
      setState({ ...FALLBACK, error: 'Geolocation not supported', loading: false });
      return;
    }

    navigator.geolocation.getCurrentPosition(
      ({ coords }) => {
        setState({
          latitude: coords.latitude,
          longitude: coords.longitude,
          error: null,
          loading: false,
        });
      },
      (err) => {
        setState({ ...FALLBACK, error: err.message, loading: false });
      }
    );
  }, []);

  return state;
}
