import { useQuery } from '@tanstack/react-query';
import api from '../services/api';
import type { MyListing } from '../types/api';

export function useMyListings() {
  return useQuery({
    queryKey: ['my-listings'],
    queryFn: async () => {
      const { data } = await api.get<MyListing[]>('/api/my-listings');
      return data;
    },
    staleTime: 30_000,
  });
}
