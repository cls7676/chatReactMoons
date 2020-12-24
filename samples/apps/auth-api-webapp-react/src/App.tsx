// Copyright (c) Microsoft. All rights reserved.

import { AuthenticatedTemplate, useAccount, useIsAuthenticated, useMsal } from '@azure/msal-react';
import { Subtitle1, Tab, TabList } from '@fluentui/react-components';
import { FC, useEffect, useState } from 'react';
import FunctionProbe from './components/FunctionProbe';
import InteractWithGraph from './components/InteractWithGraph';
import QuickTips, { ITipGroup } from './components/QuickTips';
import ServiceConfig from './components/ServiceConfig';
import YourInfo from './components/YourInfo';
import { IKeyConfig } from './model/KeyConfig';

const App: FC = () => {
    enum AppState {
        ProbeForFunction = 0,
        YourInfo = 1,
        Setup = 2,
        InteractWithGraph = 3,
    }

    const isAuthenticated = useIsAuthenticated();
    const { instance, accounts } = useMsal();
    const account = useAccount(accounts[0] || {});
    const [appState, setAppState] = useState<AppState>(AppState.ProbeForFunction);
    const [selectedTabValue, setSelectedTabValue] = useState<string>(isAuthenticated ? 'setup' : 'yourinfo');
    const [config, setConfig] = useState<IKeyConfig>();

    const appStateToTabValueMap = new Map<AppState, string>([
        [AppState.Setup, 'setup'],
        [AppState.InteractWithGraph, 'interact'],
        [AppState.YourInfo, 'yourinfo'],
    ]);
    const tabValueToAppStateMap = new Map<string, AppState>([
        ['setup', AppState.Setup],
        ['yourinfo', AppState.YourInfo],
        ['interact', AppState.InteractWithGraph],
    ]);

    useEffect(() => {
        changeAppState(appState);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [appState]);

    useEffect(() => {
        if (isAuthenticated) {
            setAppState(AppState.Setup);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isAuthenticated]);

    const changeAppState = function (newAppState: AppState) {
        setAppState(newAppState);
        setSelectedTabValue(appStateToTabValueMap.get(newAppState) ?? 'setup');
    };
    const changeTabValue = function (newTabValue: string) {
        setSelectedTabValue(newTabValue);
        setAppState(t