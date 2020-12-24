// Copyright (c) Microsoft. All rights reserved.

import { Body1, Spinner, Title3 } from '@fluentui/react-components';
import { FC, useEffect } from 'react';

interface IData {
    uri: string;
    onFunctionFound: () => void;
}

const FunctionProbe: FC<IData> = ({ uri, onFunctionFound }) => {
    useEffect(() => {
        const fetchAsync = async () => {
            try {
       