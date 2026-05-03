import { useEffect, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { X } from 'lucide-react';
import { useParams } from 'react-router-dom';
import { Button } from '../ui/Button';
import { Textarea } from '../ui/Textarea';
import { useClaimListing } from '../../hooks/useClaimListing';

const schema = z.object({
  notes: z.string().max(500, 'Notes must be 500 characters or fewer'),
});

type FormValues = z.infer<typeof schema>;

interface ClaimModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function ClaimModal({ isOpen, onClose }: ClaimModalProps) {
  const { id: listingId = '' } = useParams<{ id: string }>();
  const { mutate: claimListing, isPending, isSuccess, error } = useClaimListing();
  const overlayRef = useRef<HTMLDivElement>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { notes: '' },
  });

  useEffect(() => {
    if (isSuccess) {
      reset();
      onClose();
    }
  }, [isSuccess, reset, onClose]);

  useEffect(() => {
    if (!isOpen) reset();
  }, [isOpen, reset]);

  useEffect(() => {
    const handleKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };
    if (isOpen) document.addEventListener('keydown', handleKey);
    return () => document.removeEventListener('keydown', handleKey);
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const onSubmit = (values: FormValues) => {
    claimListing({ listingId, notes: values.notes });
  };

  const handleOverlayClick = (e: React.MouseEvent) => {
    if (e.target === overlayRef.current) onClose();
  };

  return (
    <div
      ref={overlayRef}
      onClick={handleOverlayClick}
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4"
    >
      <div className="w-full max-w-md rounded-xl bg-white shadow-xl">
        <div className="flex items-center justify-between border-b border-gray-200 px-6 py-4">
          <h2 className="text-lg font-semibold text-gray-900">Claim this listing</h2>
          <button
            onClick={onClose}
            className="rounded-lg p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600 transition-colors"
            aria-label="Close"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="px-6 py-5 space-y-4">
          <p className="text-sm text-gray-600">
            Let the donor know when you plan to collect and any relevant details.
          </p>

          <Textarea
            label="Notes (optional)"
            rows={4}
            placeholder="e.g. I can collect tomorrow afternoon between 2–4pm."
            error={errors.notes?.message}
            {...register('notes')}
          />

          {error && (
            <p className="text-sm text-red-600">
              {(error as { response?: { data?: { message?: string } } })?.response?.data?.message ??
                'Something went wrong. Please try again.'}
            </p>
          )}

          <div className="flex justify-end gap-3 pt-1">
            <Button type="button" variant="secondary" onClick={onClose} disabled={isPending}>
              Cancel
            </Button>
            <Button type="submit" disabled={isPending}>
              {isPending ? 'Claiming…' : 'Confirm claim'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
