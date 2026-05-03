import { useQuery } from '@tanstack/react-query';
import api from '../services/api';
import type { NearbyListing } from '../types/api';

interface UseNearbyListingsParams {
  latitude: number | null;
  longitude: number | null;
  radiusKm?: number;
}

export function useNearbyListings({ latitude, longitude, radiusKm = 10 }: UseNearbyListingsParams) {
  return useQuery({
    queryKey: ['nearby-listings', latitude, longitude, radiusKm],
    queryFn: async () => {
      const { data } = await api.get<NearbyListing[]>('/api/listings/nearby', {
        params: { latitude, longitude, radiusKm },
      });
      return data;
    },
    enabled: latitude !== null && longitude !== null,
    staleTime: 30_000,
  });
}
