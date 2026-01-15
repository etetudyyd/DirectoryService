"use client";

import { AppSidebar } from "@/shared/components/sidebar/app-sidebar";
import { SidebarProvider, SidebarInset } from "@/shared/components/ui/sidebar";
import { QueryClientProvider } from "@tanstack/react-query";
import Header from "../header/header";
import { queryClient } from "@/shared/api/query-client";

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>
        <SidebarProvider>
          <div className="flex min-h-screen w-full">
            <AppSidebar />

            <div className="flex-1 min-w-0 flex flex-col">
              <Header />
              <SidebarInset className="max-w-7xl mx-auto p-6">{children}</SidebarInset>
            </div>
          </div>
        </SidebarProvider>
        </QueryClientProvider>
        );
    }