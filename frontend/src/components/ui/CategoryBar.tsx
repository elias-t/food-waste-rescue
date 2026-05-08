interface CategoryBarProps {
  category: string;
  count: number;
  maxCount: number;
}

function formatCategoryName(key: string): string {
  return key.replace(/([A-Z])/g, ' $1').trim();
}

export function CategoryBar({ category, count, maxCount }: CategoryBarProps) {
  const pct = maxCount > 0 ? (count / maxCount) * 100 : 0;

  return (
    <div className="flex items-center gap-3">
      <span className="w-36 shrink-0 text-sm text-slate-700 truncate">
        {formatCategoryName(category)}
      </span>
      <div className="flex-1 bg-gray-100 rounded-full h-2 overflow-hidden">
        <div
          className="bg-primary h-2 rounded-full transition-all duration-500"
          style={{ width: `${pct}%` }}
        />
      </div>
      <span className="w-8 text-right text-sm font-medium text-slate-700 shrink-0">
        {count}
      </span>
    </div>
  );
}
