import { Link } from 'react-router-dom';
import { Package, Heart, Clock, Trophy } from 'lucide-react';
import { useImpactReport } from '../hooks/useImpactReport';
import { StatCard } from '../components/ui/StatCard';
import { CategoryBar } from '../components/ui/CategoryBar';
import { Button } from '../components/ui/Button';

function LoadingSkeleton() {
  return (
    <div className="max-w-4xl mx-auto space-y-10 animate-pulse">
      <div className="text-center space-y-3 pt-4">
        <div className="h-8 bg-gray-200 rounded w-64 mx-auto" />
        <div className="h-4 bg-gray-200 rounded w-80 mx-auto" />
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
        {Array.from({ length: 3 }).map((_, i) => (
          <div key={i} className="bg-white rounded-xl p-6 shadow-sm space-y-3">
            <div className="h-10 w-10 bg-gray-200 rounded-lg" />
            <div className="h-10 bg-gray-200 rounded w-20" />
            <div className="h-4 bg-gray-200 rounded w-28" />
          </div>
        ))}
      </div>
      <div className="bg-white rounded-xl p-6 shadow-sm space-y-4">
        <div className="h-5 bg-gray-200 rounded w-48" />
        {Array.from({ length: 4 }).map((_, i) => (
          <div key={i} className="h-4 bg-gray-200 rounded" />
        ))}
      </div>
    </div>
  );
}

export default function ImpactPage() {
  const { data, isLoading, isError, refetch } = useImpactReport();

  if (isLoading) return <LoadingSkeleton />;

  if (isError || !data) {
    return (
      <div className="max-w-4xl mx-auto py-16 text-center space-y-4">
        <p className="text-slate-600">Unable to load impact data.</p>
        <Button variant="secondary" onClick={() => refetch()}>Try again</Button>
      </div>
    );
  }

  const categoryEntries = Object.entries(data.foodSavedByCategory)
    .sort(([, a], [, b]) => b - a);
  const maxCount = categoryEntries.length > 0
    ? Math.max(...categoryEntries.map(([, n]) => n))
    : 0;

  return (
    <div className="max-w-4xl mx-auto">
      {/* Hero */}
      <div className="text-center pt-4">
        <span className="text-5xl">🌱</span>
        <h1 className="text-3xl font-bold text-slate-900 mt-3">Our Community Impact</h1>
        <p className="text-slate-600 mt-2">Every listing saved is food that didn't go to waste</p>
        <p className="text-sm text-slate-400 mt-1">
          Last updated: {new Date().toLocaleDateString()}
        </p>
      </div>

      {/* Stats grid */}
      <div className="mt-10 grid grid-cols-1 sm:grid-cols-3 gap-6">
        <StatCard
          value={data.totalListings}
          label="Listings Posted"
          icon={<Package className="h-5 w-5" />}
          colour="blue"
        />
        <StatCard
          value={data.totalClaimed}
          label="Meals Rescued"
          icon={<Heart className="h-5 w-5" />}
          colour="green"
        />
        <StatCard
          value={data.totalExpired}
          label="Listings Expired"
          icon={<Clock className="h-5 w-5" />}
          colour="red"
        />
      </div>

      {/* Most active donor */}
      {data.mostActiveDonor && (
        <div className="mt-10 bg-white rounded-xl shadow-sm p-6 flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-yellow-100 flex items-center justify-center shrink-0">
            <Trophy className="h-6 w-6 text-yellow-600" />
          </div>
          <div>
            <p className="text-sm font-medium text-slate-500">Most Active Donor</p>
            <p className="text-xl font-bold text-primary">{data.mostActiveDonor}</p>
            <p className="text-sm text-slate-500">Thank you for making a difference!</p>
          </div>
        </div>
      )}

      {/* Food by category */}
      <div className="mt-10 bg-white rounded-xl shadow-sm p-6">
        <h2 className="text-lg font-semibold text-slate-900 mb-5">Food Rescued by Category</h2>
        {categoryEntries.length > 0 ? (
          <div className="space-y-4">
            {categoryEntries.map(([category, count]) => (
              <CategoryBar
                key={category}
                category={category}
                count={count}
                maxCount={maxCount}
              />
            ))}
          </div>
        ) : (
          <p className="text-sm text-slate-500">No data yet — be the first to donate!</p>
        )}
      </div>

      {/* CTA */}
      <div className="mt-12 text-center space-y-4 pb-8">
        <p className="text-slate-600 font-medium">Want to help reduce food waste?</p>
        <div className="flex items-center justify-center gap-3 flex-wrap">
          <Link to="/register">
            <Button size="lg">Join as a Donor</Button>
          </Link>
          <Link to="/listings">
            <Button size="lg" variant="secondary">Browse Available Food</Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
