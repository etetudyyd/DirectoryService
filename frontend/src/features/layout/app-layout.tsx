"use client";

import { AppSidebar } from "@/shared/components/sidebar/app-sidebar";
import { SidebarProvider, SidebarInset } from "@/shared/components/ui/sidebar";
import { QueryClientProvider } from "@tanstack/react-query";
import Header from "../header/header";
import { queryClient } from "@/shared/api/query-client";
import { Toaster } from "@/shared/components/ui/sonner";

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>
      <SidebarProvider>
        <div className="flex min-h-screen w-full bg-linear-to-br">
          <AppSidebar />

          <div className="flex-1 min-w-0 flex flex-col">
            <Header />

            <SidebarInset className="flex-1">
              <main className="max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
                {children}
              </main>
            </SidebarInset>

            <Toaster
              position="top-center"
              duration={3000}
              toastOptions={{
                className: "bg-gray-900 border border-gray-700 text-white",
              }}
            />
          </div>
        </div>
      </SidebarProvider>
    </QueryClientProvider>
  );
}
