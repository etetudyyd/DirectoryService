'use client';
import React from 'react';
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from '@/shared/components/ui/pagination';

type Props = {
  page: number;
  totalPages: number;
  onPageChange: (p: number) => void;
};

export default function LocationsPagination({ page, totalPages, onPageChange }: Props) {
  if (totalPages <= 1) return null;

  return (
    <div className="mt-8">
      <Pagination>
        <PaginationContent>
          <PaginationItem>
            <PaginationPrevious
              href="#"
              onClick={(e) => {
                e.preventDefault();
                if (page > 1) onPageChange(page - 1);
              }}
              className={page === 1 ? 'pointer-events-none opacity-50' : ''}
            />
          </PaginationItem>

          {Array.from({ length: totalPages }, (_, i) => i + 1).map((pageNum) => {
            const isCurrentPage = pageNum === page;
            const isNearCurrent = Math.abs(pageNum - page) <= 1;
            const isFirst = pageNum === 1;
            const isLast = pageNum === totalPages;
            const shouldShow = isFirst || isLast || isNearCurrent;

            if (!shouldShow && pageNum === 2) {
              return (
                <PaginationItem key="ellipsis-start">
                  <PaginationEllipsis />
                </PaginationItem>
              );
            }

            if (!shouldShow && pageNum === totalPages - 1) {
              return (
                <PaginationItem key="ellipsis-end">
                  <PaginationEllipsis />
                </PaginationItem>
              );
            }

            if (!shouldShow) return null;

            return (
              <PaginationItem key={pageNum}>
                <PaginationLink
                  href="#"
                  isActive={isCurrentPage}
                  onClick={(e) => {
                    e.preventDefault();
                    onPageChange(pageNum);
                  }}
                >
                  {pageNum}
                </PaginationLink>
              </PaginationItem>
            );
          })}

          <PaginationItem>
            <PaginationNext
              href="#"
              onClick={(e) => {
                e.preventDefault();
                if (page < totalPages) onPageChange(page + 1);
              }}
              className={page === totalPages ? 'pointer-events-none opacity-50' : ''}
            />
          </PaginationItem>
        </PaginationContent>
      </Pagination>
    </div>
  );
}
