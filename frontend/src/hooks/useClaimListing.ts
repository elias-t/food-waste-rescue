import { useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../services/api';

export function useClaimListing() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ listingId, notes }: { listingId: string; notes: string }) => {
      const { data } = await api.post(`/api/listings/${listingId}/claim`, { notes });
      return data;
    },
    onSuccess: (_, { listingId }) => {
      queryClient.invalidateQueries({ queryKey: ['listing-detail', listingId] });
      queryClient.invalidateQueries({ queryKey: ['nearby-listings'] });
      queryClient.invalidateQueries({ queryKey: ['my-listings'] });
    },
  });
}
