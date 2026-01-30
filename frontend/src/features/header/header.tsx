"use client"

import Link from "next/link";
import { Search, Bell, Settings, User, X, Building2 } from "lucide-react";
import routes from "@/shared/routes";
import { SidebarTrigger } from "@/shared/components/ui/sidebar";
import { Button } from "@/shared/components/ui/button";
import { Input } from "@/shared/components/ui/input";
import { Badge } from "@/shared/components/ui/badge";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/shared/components/ui/sheet";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/shared/components/ui/dropdown-menu";
import { useState } from "react";
import { setGlobalSearch, useGetGlobalSearch } from "@/shared/stored/global-search-store";

export default function Header() {

  const { search } = useGetGlobalSearch();
  const [searchOpen, setSearchOpen] = useState(false);
  

  return (
    <>
      <header className="sticky top-0 z-40 w-full h-16 border-slate-700/50 bg-linear-to-br from-slate-900/50 to-slate-800/30 backdrop-blur supports-backdrop-filter:bg-background/60 border-b flex items-center px-4 sm:px-6 lg:px-8">
        <div className="flex items-center gap-3">
          <SidebarTrigger className="md:hidden" />
          
          <div className="hidden md:flex h-10 w-10 items-center justify-center rounded-lg bg-linear-to-br from-primary to-primary/80 text-primary-foreground font-bold shadow-lg">
            <Building2 className="h-5 w-5" />
          </div>
          
          <Link href={routes.home} className="group">
            <h1 className="text-lg font-semibold tracking-tight group-hover:text-primary transition-colors">
              Directory Service
            </h1>
            <div className="text-xs text-muted-foreground group-hover:text-foreground/80 transition-colors">
              Management web-service
            </div>
          </Link>
        </div>

        <div className="flex-1" />
        
        <div className="flex items-center gap-2">
          {/* Desktop Search */}
          <div className="hidden md:flex items-center relative max-w-xs w-full">
            <Search className="absolute left-3 h-4 w-4 text-muted-foreground" />
            <Input
              type="search"
              placeholder="Search..."
              className="pl-9 bg-background border-input w-full focus-visible:ring-primary/20"
              value={search}
              onChange={(e) => setGlobalSearch(e.target.value)}
            />
          </div>

          {/* Mobile Search Toggle */}
          <Sheet open={searchOpen} onOpenChange={setSearchOpen}>
            <SheetTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                className="md:hidden"
                aria-label={searchOpen ? "Close search" : "Open search"}
              >
                <Search className="h-5 w-5" />
              </Button>
            </SheetTrigger>
            <SheetContent side="top" className="pt-14">
              <SheetHeader>
                <SheetTitle>Search</SheetTitle>
              </SheetHeader>
              <div className="py-4">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    type="search"
                    placeholder="Search locations, departments..."
                    className="pl-9"
                    autoFocus
                    value={search}
                    onChange={(e) => setGlobalSearch(e.target.value)}
                  />
                </div>
              </div>
            </SheetContent>
          </Sheet>

          <div className="flex items-center gap-1">
            {/* Notifications */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="relative">
                  <Bell className="h-5 w-5" />
                  <Badge 
                    className="absolute -top-1 -right-1 h-2 w-2 p-0 bg-destructive border-destructive" 
                  />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-64">
                <DropdownMenuLabel>Notifications</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <div className="max-h-64 overflow-y-auto p-1">
                  <DropdownMenuItem className="cursor-pointer">
                    <div className="space-y-1">
                      <p className="text-sm font-medium">New location added</p>
                      <p className="text-xs text-muted-foreground">2 minutes ago</p>
                    </div>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem className="cursor-pointer">
                    <div className="space-y-1">
                      <p className="text-sm font-medium">System update completed</p>
                      <p className="text-xs text-muted-foreground">1 hour ago</p>
                    </div>
                  </DropdownMenuItem>
                </div>
              </DropdownMenuContent>
            </DropdownMenu>

            {/* Settings */}
            <Button asChild variant="ghost" size="icon">
              <Link href="/settings">
                <Settings className="h-5 w-5" />
              </Link>
            </Button>

            {/* User profile - desktop */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="hidden md:flex items-center gap-2 pl-2">
                  <div className="h-8 w-8 rounded-full bg-linear-to-br from-muted to-muted/80 border flex items-center justify-center">
                    <User className="h-4 w-4" />
                  </div>
                  <div className="text-left hidden lg:block">
                    <div className="text-sm font-medium">Admin User</div>
                    <div className="text-xs text-muted-foreground">Administrator</div>
                  </div>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuLabel>My Account</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem>Profile</DropdownMenuItem>
                <DropdownMenuItem>Settings</DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="text-destructive">
                  Log out
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>

            {/* User profile - mobile */}
            <Button variant="ghost" size="icon" className="md:hidden">
              <User className="h-5 w-5" />
            </Button>
          </div>
        </div>
      </header>

      {/* Legacy mobile search dropdown (fallback) */}
      {searchOpen && (
        <div className="md:hidden fixed inset-x-0 top-16 z-30 bg-background border-b px-4 py-3 shadow-lg">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              type="search"
              placeholder="Search locations, departments..."
              className="pl-9"
              autoFocus
              value={search}
              onChange={(e) => setGlobalSearch(e.target.value)}
            />
          </div>
        </div>
      )}
    </>
  );
}